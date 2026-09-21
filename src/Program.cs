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
using System.Threading;
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
 public static bool Valid(Catalog c){return c!=null&&!string.IsNullOrWhiteSpace(c.repository)&&!string.IsNullOrWhiteSpace(c.@ref)&&!string.IsNullOrWhiteSpace(c.licenseApi)&&c.actions!=null&&c.actions.Length>=100&&c.files!=null&&c.files.Count>100;}
 public static async Task<Catalog> LoadLatest(Catalog embedded){
  if(!Valid(embedded))throw new InvalidDataException("Catálogo interno inválido.");
  try{
   using(var client=new HttpClient{Timeout=TimeSpan.FromSeconds(20)}){
    client.DefaultRequestHeaders.Add("User-Agent","ice-optimizer/"+(embedded.version??"app"));
    string commitsUrl="https://api.github.com/repos/"+embedded.repository+"/commits?path=catalog.json&per_page=1&ice="+DateTime.UtcNow.Ticks;
    string commitsJson=await client.GetStringAsync(commitsUrl);
    var commits=new JavaScriptSerializer().DeserializeObject(commitsJson) as object[];
    if(commits==null||commits.Length==0)throw new InvalidDataException("Nenhuma versão publicada foi encontrada.");
    var first=commits[0] as Dictionary<string,object>;
    if(first==null||!first.ContainsKey("sha"))throw new InvalidDataException("Referência publicada inválida.");
    string sha=Convert.ToString(first["sha"]);
    if(string.IsNullOrWhiteSpace(sha)||sha.Length<20)throw new InvalidDataException("SHA publicado inválido.");
    string url="https://raw.githubusercontent.com/"+embedded.repository+"/"+sha+"/catalog.json";
    string json=await client.GetStringAsync(url);
    var latest=new JavaScriptSerializer().Deserialize<Catalog>(json);
    if(!Valid(latest)||!string.Equals(latest.repository,embedded.repository,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Catálogo online inválido.");
    latest.@ref=sha;
    return latest;
   }
  }catch(Exception ex){throw new IOException("Não foi possível obter a versão verificada do Ice Optimizer. Confira sua conexão e tente novamente.\n\n"+ex.Message,ex);}
 }}
public static class Payload {
 static string onlineRoot=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"ice optimizer","online-scripts");
 public static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();}
 public static void Verify(byte[] bytes, FileInfoEntry entry){if(bytes.LongLength!=entry.size||Hash(bytes)!=entry.sha256)throw new InvalidDataException("Os arquivos online mudaram durante a sincronização. Feche e abra o Ice Optimizer novamente. Se continuar, baixe a versão mais recente no site.");}
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

 public static async Task RefreshOnline(Catalog cat,string token,Action<string,int,int> progress,string targetRoot=null){
  string finalRoot=Path.GetFullPath(targetRoot??onlineRoot),allowed=Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"ice optimizer"));
  if(targetRoot==null&&!finalRoot.StartsWith(allowed+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Cache online inválido.");
  string staging=finalRoot+".new-"+Guid.NewGuid().ToString("N");Directory.CreateDirectory(staging);var remotes=cat.files.Keys.Where(x=>x.StartsWith("scripts/",StringComparison.OrdinalIgnoreCase)).OrderBy(x=>x).ToArray();int done=0;var gate=new SemaphoreSlim(6);
  try{
   var tasks=remotes.Select(async remote=>{await gate.WaitAsync();try{byte[] bytes=await Download(cat,remote,token);string dest=Destination(staging,remote);Directory.CreateDirectory(Path.GetDirectoryName(dest));File.WriteAllBytes(dest,bytes);int current=Interlocked.Increment(ref done);progress(Path.GetFileName(remote),current,remotes.Length);}finally{gate.Release();}}).ToArray();
   await Task.WhenAll(tasks);if(Directory.Exists(finalRoot))Directory.Delete(finalRoot,true);Directory.Move(staging,finalRoot);onlineRoot=finalRoot;
  }catch{try{if(Directory.Exists(staging))Directory.Delete(staging,true);}catch{}throw;}finally{gate.Dispose();}
 }
 public static Task<string> Prepare(Catalog cat,ActionItem action,string root,string token,Action<string> report){
  root=SafeRoot(root);string run=Path.Combine(root,DateTime.Now.ToString("yyyyMMdd-HHmmss")+"-"+Guid.NewGuid().ToString("N").Substring(0,8));Directory.CreateDirectory(run);
  foreach(string remote in new[]{action.file}.Concat(action.dependencies)){
   report("Preparando: "+Path.GetFileName(remote));string source=Destination(onlineRoot,remote);if(!File.Exists(source))throw new FileNotFoundException("Cache online incompleto. Feche e abra o aplicativo novamente: "+remote);byte[] bytes=File.ReadAllBytes(source);Verify(bytes,cat.files[remote]);
   string dest=Destination(run,remote);Directory.CreateDirectory(Path.GetDirectoryName(dest));File.WriteAllBytes(dest,bytes);
  }
  return Task.FromResult(Destination(run,action.file));
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
    var cat=Catalog.Load();string cache=Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[1])),"online-cache");Payload.RefreshOnline(cat,null,(a,b,c)=>{},cache).GetAwaiter().GetResult();foreach(string id in new[]{"priorizar_rust","opcao22"}){var a=cat.actions.Single(x=>x.id==id);Payload.Prepare(cat,a,Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[1])),"download-test"),null,x=>{}).GetAwaiter().GetResult();}
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
    if(args.Length>1)File.WriteAllText(args[1],"PASS: clean UI boundary; trusted catalog and descriptions; hash tampering rejection; unsafe paths; queue order, cancellation, error policies, conflicts and restore ordering; harmless process output/exit code; online-cache boundary. No optimization executed.");return 0;
   }
   if(args.Length==2&&args[0]=="--render-login"){using(var preview=new AccountForm(Catalog.Load(),"")){preview.ShowInTaskbar=false;preview.Show();preview.Refresh();Application.DoEvents();using(var bmp=new Bitmap(preview.Width,preview.Height)){preview.DrawToBitmap(bmp,new Rectangle(0,0,preview.Width,preview.Height));bmp.Save(args[1]);}preview.Hide();return 0;}}
    if(args.Length==2&&args[0].StartsWith("--render")){using(var preview=new MainWindow()){string mode=args[0].StartsWith("--render-")?args[0].Substring(9):"";preview.Render(args[1],mode);return 0;}}
    if(!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)){try{Process.Start(new ProcessStartInfo(Application.ExecutablePath){UseShellExecute=true,Verb="runas"});}catch(System.ComponentModel.Win32Exception){MessageBox.Show("O Ice Optimizer precisa ser aberto como administrador.","Permissão necessária",MessageBoxButtons.OK,MessageBoxIcon.Information);}return 3;}
   var embedded=Catalog.Load();var catalog=Catalog.LoadLatest(embedded).GetAwaiter().GetResult();if(!LicenseGate.Ensure(catalog))return 2;if(!ScriptSyncForm.Sync(catalog))return 4;
   using(var window=new MainWindow(catalog))Application.Run(window);return 0;
  }catch(Exception e){if(args.Length>1)File.WriteAllText(args[args.Length-1],e.ToString());else MessageBox.Show(e.Message,"ice optimizer");return 1;}
 }
}
