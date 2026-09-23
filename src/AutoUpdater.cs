using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

public sealed class IceUpdateManifest {
 public string version,repository,path,sha256;
 public long size;
}
public sealed class IceUpdateRequest {
 public string target,candidate,sha256;
 public int pid;
}
public static class AutoUpdater {
 static readonly string Root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"ice optimizer","updates");
 const string ManifestPath="update.json";
 static string Quote(string value){return "\""+String(value??"").Replace("\"","\\\"")+"\"";}
 static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
 static string HashFile(string file){return Hash(File.ReadAllBytes(file));}
 static bool Newer(string current,string latest){Version a,b;return Version.TryParse(current,out a)&&Version.TryParse(latest,out b)&&b>a;}
 static bool SafeUpdatePath(string file){
  if(string.IsNullOrWhiteSpace(file))return false;
  string root=Path.GetFullPath(Root).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,full=Path.GetFullPath(file);
  return full.StartsWith(root,StringComparison.OrdinalIgnoreCase);
 }
 static void ValidateManifest(IceUpdateManifest manifest,Catalog embedded){
  if(manifest==null||!Newer(embedded.version,manifest.version))throw new InvalidOperationException("Nenhuma atualização nova.");
  if(!string.Equals(manifest.repository,embedded.repository,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Manifesto de atualização inválido.");
  if(string.IsNullOrWhiteSpace(manifest.path)||manifest.path!="downloads/ice-optimizer.exe")throw new InvalidDataException("Caminho de atualização inválido.");
  if(string.IsNullOrWhiteSpace(manifest.sha256)||manifest.sha256.Length!=64)throw new InvalidDataException("Hash de atualização inválido.");
  if(manifest.size<200000||manifest.size>50000000)throw new InvalidDataException("Tamanho de atualização inválido.");
 }
 public static bool CheckAndApply(){
  try{
   var embedded=Catalog.Load();
   using(var client=new HttpClient{Timeout=TimeSpan.FromSeconds(30)}){
    client.DefaultRequestHeaders.Add("User-Agent","ice-optimizer/"+embedded.version);
    string commitsUrl="https://api.github.com/repos/"+embedded.repository+"/commits?path="+ManifestPath+"&per_page=1&ice="+DateTime.UtcNow.Ticks;
    string commitsJson=client.GetStringAsync(commitsUrl).GetAwaiter().GetResult();
    var commits=new JavaScriptSerializer().DeserializeObject(commitsJson) as object[];
    if(commits==null||commits.Length==0)return false;
    var first=commits[0] as Dictionary<string,object>;if(first==null||!first.ContainsKey("sha"))return false;
    string commit=Convert.ToString(first["sha"]);if(string.IsNullOrWhiteSpace(commit)||commit.Length<20)return false;
    string manifestUrl="https://raw.githubusercontent.com/"+embedded.repository+"/"+commit+"/"+ManifestPath;
    var manifest=new JavaScriptSerializer().Deserialize<IceUpdateManifest>(client.GetStringAsync(manifestUrl).GetAwaiter().GetResult());
    if(manifest==null||!Newer(embedded.version,manifest.version))return false;
    ValidateManifest(manifest,embedded);
    string downloadUrl="https://raw.githubusercontent.com/"+embedded.repository+"/"+commit+"/"+manifest.path;
    byte[] bytes=client.GetByteArrayAsync(downloadUrl).GetAwaiter().GetResult();
    if(bytes.LongLength!=manifest.size||bytes.Length<2||bytes[0]!=(byte)'M'||bytes[1]!=(byte)'Z'||!string.Equals(Hash(bytes),manifest.sha256,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("A atualização baixada falhou na verificação de integridade.");
    Directory.CreateDirectory(Root);
    string id=Guid.NewGuid().ToString("N"),candidate=Path.Combine(Root,"ice-optimizer-"+manifest.version+"-"+id+".exe"),requestPath=Path.Combine(Root,"pending-"+id+".json");
    File.WriteAllBytes(candidate,bytes);
    if(!string.Equals(HashFile(candidate),manifest.sha256,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("A atualização salva falhou na verificação de integridade.");
    var request=new IceUpdateRequest{target=Path.GetFullPath(Application.ExecutablePath),candidate=Path.GetFullPath(candidate),sha256=manifest.sha256,pid=Process.GetCurrentProcess().Id};
    File.WriteAllText(requestPath,new JavaScriptSerializer().Serialize(request),new UTF8Encoding(false));
    Process.Start(new ProcessStartInfo(candidate,"--apply-update "+Quote(requestPath)){UseShellExecute=true,WorkingDirectory=Path.GetDirectoryName(candidate)});
    return true;
   }
  }catch(Exception ex){
   Debug.WriteLine("Auto-update ignorado: "+ex.Message);
   return false;
  }
 }
 public static int ApplyPending(string requestPath){
  try{
   requestPath=Path.GetFullPath(requestPath);if(!SafeUpdatePath(requestPath)||!File.Exists(requestPath))return 10;
   var request=new JavaScriptSerializer().Deserialize<IceUpdateRequest>(File.ReadAllText(requestPath,Encoding.UTF8));
   if(request==null||!SafeUpdatePath(request.candidate)||!string.Equals(Path.GetFullPath(request.candidate),Path.GetFullPath(Application.ExecutablePath),StringComparison.OrdinalIgnoreCase))return 11;
   if(string.IsNullOrWhiteSpace(request.target)||!request.target.EndsWith(".exe",StringComparison.OrdinalIgnoreCase)||!File.Exists(request.target))return 12;
   var targetInfo=FileVersionInfo.GetVersionInfo(request.target);if(!string.Equals(targetInfo.ProductName,"Ice Optimizer",StringComparison.OrdinalIgnoreCase))return 12;
   if(string.IsNullOrWhiteSpace(request.sha256)||request.sha256.Length!=64||!string.Equals(HashFile(Application.ExecutablePath),request.sha256,StringComparison.OrdinalIgnoreCase))return 13;
   try{using(var old=Process.GetProcessById(request.pid)){if(!old.HasExited)old.WaitForExit(30000);}}catch(ArgumentException){}
   for(int i=0;i<20;i++){try{File.Copy(Application.ExecutablePath,request.target,true);break;}catch(IOException){if(i==19)throw;Thread.Sleep(250);}catch(UnauthorizedAccessException){if(i==19)throw;Thread.Sleep(250);}}
   if(!string.Equals(HashFile(request.target),request.sha256,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Falha ao validar o executável instalado.");
   Process.Start(new ProcessStartInfo(request.target,"--cleanup-update "+Quote(requestPath)+" "+Quote(Application.ExecutablePath)){UseShellExecute=true,WorkingDirectory=Path.GetDirectoryName(request.target)});
   return 0;
  }catch(Exception ex){MessageBox.Show("Não foi possível concluir a atualização automática.\n\n"+ex.Message,"Ice Optimizer",MessageBoxButtons.OK,MessageBoxIcon.Warning);return 14;}
 }
 public static void Cleanup(string[] files){
  foreach(string file in files){try{if(SafeUpdatePath(file)&&File.Exists(file))File.Delete(file);}catch{}}
 }
 public static void SelfTest(){
  if(!Newer("3.0.3","3.0.4")||Newer("3.0.4","3.0.4")||Newer("3.1.0","3.0.9"))throw new Exception("Auto-update version comparison failed");
  string inside=Path.Combine(Root,"test.tmp");if(!SafeUpdatePath(inside))throw new Exception("Auto-update path validation failed");
 }
}
