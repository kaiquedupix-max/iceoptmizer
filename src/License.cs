using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

public class LicenseState {public string token,username,plan,expiresAt,createdAt,lastValidated;}
public static class LicenseGate {
 static readonly string folder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"ice optimizer"),file=Path.Combine(folder,"account.dat");
 public static LicenseState Current{get;private set;}
 internal static string Device(){string machine="";try{using(var k=Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))machine=Convert.ToString(k.GetValue("MachineGuid"));}catch{}return Fingerprint(machine+"|"+Environment.MachineName);}
 static string Wmi(string query,string property){try{using(var search=new ManagementObjectSearcher(query))foreach(ManagementObject item in search.Get()){var value=Convert.ToString(item[property]).Trim();if(value.Length>0&&value!="To Be Filled By O.E.M.")return value;}}catch{}return "";}
 internal static string Hardware(){string uuid=Wmi("SELECT UUID FROM Win32_ComputerSystemProduct","UUID"),bios=Wmi("SELECT SerialNumber FROM Win32_BIOS","SerialNumber"),board=Wmi("SELECT SerialNumber FROM Win32_BaseBoard","SerialNumber"),cpu=Wmi("SELECT ProcessorId FROM Win32_Processor","ProcessorId");return Fingerprint("ICE-HW2|"+uuid+"|"+bios+"|"+board+"|"+cpu);}
 static string Fingerprint(string value){using(var h=SHA256.Create())return Convert.ToBase64String(h.ComputeHash(Encoding.UTF8.GetBytes(value)));}
 static LicenseState Read(){try{var enc=File.ReadAllBytes(file);var raw=ProtectedData.Unprotect(enc,null,DataProtectionScope.CurrentUser);return new JavaScriptSerializer().Deserialize<LicenseState>(Encoding.UTF8.GetString(raw));}catch{return null;}}
 static void Save(LicenseState state){Directory.CreateDirectory(folder);var raw=Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(state));File.WriteAllBytes(file,ProtectedData.Protect(raw,null,DataProtectionScope.CurrentUser));}
 static LicenseState StateFrom(Dictionary<string,object> data,LicenseState state=null){state=state??new LicenseState();if(data.ContainsKey("token")&&data["token"]!=null)state.token=Convert.ToString(data["token"]);if(data.ContainsKey("username"))state.username=Convert.ToString(data["username"]);if(data.ContainsKey("plan"))state.plan=Convert.ToString(data["plan"]);state.expiresAt=data.ContainsKey("expiresAt")?Convert.ToString(data["expiresAt"]):null;if(data.ContainsKey("createdAt"))state.createdAt=Convert.ToString(data["createdAt"]);state.lastValidated=DateTime.UtcNow.ToString("o");return state;}
 internal static async Task<Dictionary<string,object>> Request(Catalog catalog,string endpoint,Dictionary<string,string> fields,string token=null){using(var c=new HttpClient{Timeout=TimeSpan.FromSeconds(15)}){c.DefaultRequestHeaders.Add("User-Agent","ice-optimizer/3.0");if(!string.IsNullOrEmpty(token))c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",token);fields["deviceId"]=Device();fields["hardwareId"]=Hardware();var body=new JavaScriptSerializer().Serialize(fields);using(var response=await c.PostAsync(catalog.licenseApi+endpoint,new StringContent(body,Encoding.UTF8,"application/json"))){var text=await response.Content.ReadAsStringAsync();var data=new JavaScriptSerializer().Deserialize<Dictionary<string,object>>(text);if(!response.IsSuccessStatusCode)throw new InvalidOperationException(data!=null&&data.ContainsKey("message")?Convert.ToString(data["message"]):"Acesso recusado.");return data;}}}
 public static bool Ensure(Catalog catalog){var state=Read();if(state!=null&&!string.IsNullOrWhiteSpace(state.token)){try{var r=Request(catalog,"/api/account/profile",new Dictionary<string,string>(),state.token).GetAwaiter().GetResult();Current=StateFrom(r,state);Save(Current);return true;}catch(HttpRequestException){DateTime last;if(DateTime.TryParse(state.lastValidated,out last)&&DateTime.UtcNow-last.ToUniversalTime()<TimeSpan.FromHours(72)&&(string.IsNullOrEmpty(state.expiresAt)||DateTime.Parse(state.expiresAt).ToUniversalTime()>DateTime.UtcNow)){Current=state;return true;}}catch(TaskCanceledException){DateTime last;if(DateTime.TryParse(state.lastValidated,out last)&&DateTime.UtcNow-last.ToUniversalTime()<TimeSpan.FromHours(72)){Current=state;return true;}}catch{}}
  return SignIn(catalog,state==null?"":state.username);}
 public static bool SignIn(Catalog catalog,string previous=""){using(var form=new AccountForm(catalog,previous)){if(form.ShowDialog()!=DialogResult.OK)return false;Current=form.State;if(form.Remember)Save(Current);else try{if(File.Exists(file))File.Delete(file);}catch{}return true;}}
 public static void Logout(){Current=null;try{if(File.Exists(file))File.Delete(file);}catch{}}
 public static string PlanName(string plan){return plan=="d30"?"30 dias":plan=="d90"?"3 meses":plan=="d180"||plan=="m6"?"6 meses":plan=="d365"?"12 meses":plan=="permanent"?"Permanente":"Licença";}
 public static string Remaining(LicenseState s){if(s==null)return "Indisponível";if(string.IsNullOrEmpty(s.expiresAt))return "Acesso permanente";DateTime end;if(!DateTime.TryParse(s.expiresAt,out end))return "Indisponível";var left=end.ToUniversalTime()-DateTime.UtcNow;if(left<=TimeSpan.Zero)return "Expirada";if(left.TotalDays>=1)return Math.Ceiling(left.TotalDays)+" dias restantes";return Math.Max(1,Math.Ceiling(left.TotalHours))+" horas restantes";}
 internal static void UsePreview(){if(Current==null)Current=new LicenseState{username="kaique",plan="d30",expiresAt=DateTime.UtcNow.AddDays(30).ToString("o"),createdAt=DateTime.UtcNow.ToString("o"),lastValidated=DateTime.UtcNow.ToString("o")};}
}
