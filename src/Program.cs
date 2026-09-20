using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

public class ActionItem {
 public string id, title, category, warning, file, description, risk;
 public bool standalone,restart;
 public string[] conflicts;
 public string[] dependencies;
 public override string ToString(){return title;}
}
public class FileInfoEntry { public string sha256; public long size; }
public class Catalog {
 public string name, author, version, repository, @ref, licenseApi;
 public ActionItem[] actions;
 public Dictionary<string,FileInfoEntry> files;
 public static Catalog Load(){using(var s=Assembly.GetExecutingAssembly().GetManifestResourceStream("catalog.json"))using(var r=new StreamReader(s))return new JavaScriptSerializer().Deserialize<Catalog>(r.ReadToEnd());}
}
public static class Payload {
 public static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();}
 public static void Verify(byte[] bytes, FileInfoEntry entry){if(bytes.LongLength!=entry.size||Hash(bytes)!=entry.sha256)throw new InvalidDataException("Integridade inválida. O arquivo não será executado. Atualize o aplicativo com o responsável.");}
 public static string SafeRoot(string root){
  if(string.IsNullOrWhiteSpace(root)||root.IndexOfAny(new[]{'"','%','!','\r','\n','&','^','|','<','>'})>=0)throw new ArgumentException("Escolha uma pasta local sem caracteres especiais de comando.");
  root=Path.GetFullPath(root);if(root.StartsWith(@"\\"))throw new ArgumentException("Escolha uma pasta em disco local.");return root;
 }
 public static string Destination(string root,string remote){
  if(!remote.StartsWith("scripts/")||remote.Contains("..")||remote.Contains(":"))throw new InvalidDataException("Caminho não permitido.");
  string rel=remote.StartsWith("scripts/actions/")?remote.Substring(16):remote.Substring(8);
  string full=Path.GetFullPath(Path.Combine(root,rel));
  if(!full.StartsWith(Path.GetFullPath(root).TrimEnd('\\')+"\\",StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Caminho fora da pasta de execução.");return full;
 }
 public static async Task<byte[]> Download(Catalog cat,string remote,string token){
  using(var handler=new HttpClientHandler{AllowAutoRedirect=false})using(var client=new HttpClient(handler)){
   client.Timeout=TimeSpan.FromSeconds(90);
   client.DefaultRequestHeaders.Add("User-Agent","ice-optimizer/1.0");
   string url="https://raw.githubusercontent.com/"+cat.repository+"/"+cat.@ref+"/"+string.Join("/",remote.Split('/').Select(Uri.EscapeDataString));
   using(var response=await client.GetAsync(url)){
    if(!response.IsSuccessStatusCode)throw new IOException("Download: HTTP "+(int)response.StatusCode+". Confira sua conexão e a disponibilidade do repositório.");
    byte[] bytes=await response.Content.ReadAsByteArrayAsync();Verify(bytes,cat.files[remote]);return bytes;
   }
  }
 }

 public static async Task<string> Prepare(Catalog cat,ActionItem action,string root,bool local,string token,Action<string> report){
  root=SafeRoot(root);string run=Path.Combine(root,DateTime.Now.ToString("yyyyMMdd-HHmmss")+"-"+Guid.NewGuid().ToString("N").Substring(0,8));Directory.CreateDirectory(run);
  foreach(string remote in new[]{action.file}.Concat(action.dependencies)){
   report("Preparando: "+Path.GetFileName(remote));byte[] bytes;
   if(local){string p=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,remote.Replace('/',Path.DirectorySeparatorChar));if(!File.Exists(p))throw new FileNotFoundException("Arquivo ausente no pacote offline: "+remote);bytes=File.ReadAllBytes(p);Verify(bytes,cat.files[remote]);}
   else bytes=await Download(cat,remote,token);
   string dest=Destination(run,remote);Directory.CreateDirectory(Path.GetDirectoryName(dest));File.WriteAllBytes(dest,bytes);
  }
  return Destination(run,action.file);
 }
 public static Task<int> Execute(string script,Action<string> log){return Task.Run(()=>{
  var info=new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),"cmd.exe"),"/d /s /c \"\""+script+"\"\""){
   UseShellExecute=false,CreateNoWindow=true,WorkingDirectory=Path.GetDirectoryName(script),RedirectStandardOutput=true,RedirectStandardError=true,RedirectStandardInput=true,StandardOutputEncoding=Encoding.UTF8,StandardErrorEncoding=Encoding.UTF8
  };
  // Never pass the GitHub credential to the child process.
  using(var process=new Process{StartInfo=info}){
   process.OutputDataReceived+=(s,e)=>{if(e.Data!=null)log(e.Data);};process.ErrorDataReceived+=(s,e)=>{if(e.Data!=null)log(e.Data);};
   process.Start();process.StandardInput.Close();process.BeginOutputReadLine();process.BeginErrorReadLine();process.WaitForExit();return process.ExitCode;
  }
 });}
}

public static class Program {
 [STAThread] public static int Main(string[] args){
  ServicePointManager.SecurityProtocol=SecurityProtocolType.Tls12;Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
  try{
   if(args.Length==2&&args[0]=="--download-test"){
    var cat=Catalog.Load();foreach(string id in new[]{"priorizar_rust","opcao22"}){var a=cat.actions.Single(x=>x.id==id);Payload.Prepare(cat,a,Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[1])),"download-test"),false,null,x=>{}).GetAwaiter().GetResult();}
    File.WriteAllText(args[1],"PASS: public HTTPS downloads and SHA-256 verification, Rust script and complete ISLC dependency tree; no script executed.");return 0;
   }
   if(args.Length>0&&args[0]=="--self-test"){
    var c=Catalog.Load();if(c.actions.Length<100||c.actions.Select(a=>a.id).Distinct().Count()!=c.actions.Length)throw new Exception("Invalid catalog");
    byte[] b=Encoding.UTF8.GetBytes("verified");var f=new FileInfoEntry{sha256=Payload.Hash(b),size=b.Length};Payload.Verify(b,f);bool rejected=false;try{Payload.Verify(new byte[]{1},f);}catch(InvalidDataException){rejected=true;}if(!rejected)throw new Exception("Corruption accepted");
    foreach(string p in new[]{"C:\\bad%PATH%","C:\\bad!name","\\\\server\\share"}){rejected=false;try{Payload.SafeRoot(p);}catch(ArgumentException){rejected=true;}if(!rejected)throw new Exception("Unsafe path accepted");}
    using(var window=new MainWindow())window.UiTest();
    Verification.BatchTests().GetAwaiter().GetResult();
    string testRoot=Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[1])),"test runs "+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(testRoot);
    string script=Path.Combine(testRoot,"harmless.bat");File.WriteAllText(script,"@echo off\r\necho ICE_TEST_OK\r\nexit /b 7\r\n");var lines=new List<string>();int code=Payload.Execute(script,l=>{lock(lines)lines.Add(l);}).GetAwaiter().GetResult();if(code!=7||!lines.Contains("ICE_TEST_OK"))throw new Exception("Process output/exit test failed");
    var act=c.actions.Single(a=>a.id=="ping");string prepared=Payload.Prepare(c,act,testRoot,true,null,l=>{}).GetAwaiter().GetResult();if(!File.Exists(Path.Combine(Path.GetDirectoryName(prepared),"DnsJumper.exe")))throw new Exception("Dependency layout invalid");
    if(args.Length>1)File.WriteAllText(args[1],"PASS: clean UI boundary; trusted catalog and descriptions; hash tampering rejection; unsafe paths; queue order, preparation before execution, cancellation, error policies, conflicts and restore ordering; harmless process output/exit code; offline dependencies. No optimization executed.");return 0;
   }
   if(args.Length==2&&args[0]=="--render-login"){using(var preview=new AccountForm(Catalog.Load(),"")){preview.ShowInTaskbar=false;preview.Show();preview.Refresh();Application.DoEvents();using(var bmp=new Bitmap(preview.Width,preview.Height)){preview.DrawToBitmap(bmp,new Rectangle(0,0,preview.Width,preview.Height));bmp.Save(args[1]);}preview.Hide();return 0;}}
    if(args.Length==2&&args[0].StartsWith("--render")){using(var preview=new MainWindow()){if(args[0]=="--render-small")preview.Size=new Size(980,720);preview.Render(args[1],args[0]=="--render-selected"?"selected":args[0]=="--render-profile"?"profile":args[0]=="--render-progress"?"progress":args[0]=="--render-hardware"?"hardware":args[0]=="--render-games"?"games":"");return 0;}}
    if(!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)){try{Process.Start(new ProcessStartInfo(Application.ExecutablePath){UseShellExecute=true,Verb="runas"});}catch(System.ComponentModel.Win32Exception){MessageBox.Show("O Ice Optimizer precisa ser aberto como administrador.","Permissão necessária",MessageBoxButtons.OK,MessageBoxIcon.Information);}return 3;}
   var catalog=Catalog.Load();if(!LicenseGate.Ensure(catalog))return 2;
   using(var window=new MainWindow())Application.Run(window);return 0;
  }catch(Exception e){if(args.Length>1)File.WriteAllText(args[args.Length-1],e.ToString());else MessageBox.Show(e.Message,"ice optimizer");return 1;}
 }
}
