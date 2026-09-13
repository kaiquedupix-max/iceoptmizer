using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;

public class LicenseState {public string key,expiresAt,lastValidated;}
public static class LicenseGate {
 static readonly string folder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"ice optimizer"),file=Path.Combine(folder,"license.dat");
 static string Device(){string machine="";try{using(var k=Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))machine=Convert.ToString(k.GetValue("MachineGuid"));}catch{}using(var h=SHA256.Create())return Convert.ToBase64String(h.ComputeHash(Encoding.UTF8.GetBytes(machine+"|"+Environment.MachineName)));}
 static LicenseState Read(){try{var enc=File.ReadAllBytes(file);var raw=ProtectedData.Unprotect(enc,null,DataProtectionScope.CurrentUser);return new JavaScriptSerializer().Deserialize<LicenseState>(Encoding.UTF8.GetString(raw));}catch{return null;}}
 static void Save(LicenseState state){Directory.CreateDirectory(folder);var raw=Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(state));File.WriteAllBytes(file,ProtectedData.Protect(raw,null,DataProtectionScope.CurrentUser));}
 internal static async Task<Dictionary<string,object>> Request(Catalog catalog,string endpoint,string key){using(var c=new HttpClient{Timeout=TimeSpan.FromSeconds(15)}){c.DefaultRequestHeaders.Add("User-Agent","ice-optimizer/2.1");var body=new JavaScriptSerializer().Serialize(new {key=key.Trim().ToUpperInvariant(),deviceId=Device()});using(var response=await c.PostAsync(catalog.licenseApi+endpoint,new StringContent(body,Encoding.UTF8,"application/json"))){var data=new JavaScriptSerializer().Deserialize<Dictionary<string,object>>(await response.Content.ReadAsStringAsync());if(!response.IsSuccessStatusCode)throw new InvalidOperationException(data.ContainsKey("message")?Convert.ToString(data["message"]):"Licença recusada.");return data;}}}
 public static bool Ensure(Catalog catalog){var state=Read();if(state!=null&&!string.IsNullOrWhiteSpace(state.key)){try{var r=Request(catalog,"/api/license/validate",state.key).GetAwaiter().GetResult();state.expiresAt=r.ContainsKey("expiresAt")?Convert.ToString(r["expiresAt"]):null;state.lastValidated=DateTime.UtcNow.ToString("o");Save(state);return true;}catch(HttpRequestException){DateTime last;if(DateTime.TryParse(state.lastValidated,out last)&&DateTime.UtcNow-last.ToUniversalTime()<TimeSpan.FromHours(72)&&(string.IsNullOrEmpty(state.expiresAt)||DateTime.Parse(state.expiresAt).ToUniversalTime()>DateTime.UtcNow))return true;}catch(TaskCanceledException){DateTime last;if(DateTime.TryParse(state.lastValidated,out last)&&DateTime.UtcNow-last.ToUniversalTime()<TimeSpan.FromHours(72))return true;}catch{}}
  using(var form=new ActivationForm(catalog,state==null?"":state.key)){if(form.ShowDialog()!=DialogResult.OK)return false;Save(form.State);return true;}}
}
public class ActivationForm:Form {
 readonly Catalog catalog;TextBox key=new TextBox();Label message=new Label();IceButton activate=new IceButton();public LicenseState State;
 public ActivationForm(Catalog c,string previous){catalog=c;Text="Ativar Ice Optimizer";Size=new Size(610,510);StartPosition=FormStartPosition.CenterScreen;BackColor=Ice.Bg;ForeColor=Ice.Text;Font=Ice.Font(10);FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;using(var s=System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ice.ico"))if(s!=null)Icon=new Icon(s);
  var logo=new PictureBox{Image=Icon==null?null:Icon.ToBitmap(),SizeMode=PictureBoxSizeMode.Zoom};logo.SetBounds(35,31,70,70);Controls.Add(logo);var title=new Label{Text="Ative sua edição glacial",Font=Ice.Font(23,true),ForeColor=Ice.Text};title.SetBounds(125,36,430,45);Controls.Add(title);var sub=new Label{Text="Informe sua chave para liberar as 160 ações neste computador.",Font=Ice.Font(10),ForeColor=Ice.Muted};sub.SetBounds(128,83,420,34);Controls.Add(sub);
  var caption=new Label{Text="CHAVE DE LICENÇA",Font=Ice.Font(9,true),ForeColor=Ice.Cyan};caption.SetBounds(38,154,510,25);Controls.Add(caption);key.SetBounds(38,185,518,39);key.Font=Ice.Font(14);key.BackColor=Ice.Panel;key.ForeColor=Ice.Text;key.BorderStyle=BorderStyle.FixedSingle;key.Text=previous;key.CharacterCasing=CharacterCasing.Upper;Controls.Add(key);
  message.Text="A validade começa na primeira ativação. A chave fica vinculada a este computador. Depois de validada, você pode usar o aplicativo por até 72 horas sem conexão.";message.ForeColor=Ice.Muted;message.SetBounds(39,250,515,82);Controls.Add(message);
   activate.Text="Ativar e continuar";activate.Primary=true;activate.SetBounds(318,378,238,47);activate.Click+=async(s,e)=>await ActivateLicense();Controls.Add(activate);var exit=new IceButton{Text="Sair",DialogResult=DialogResult.Cancel};exit.SetBounds(38,378,130,47);Controls.Add(exit);CancelButton=exit;AcceptButton=activate;
 }
  async Task ActivateLicense(){if(key.Text.Trim().Length<10){message.ForeColor=Ice.Amber;message.Text="Digite a chave recebida na compra.";return;}activate.Enabled=false;activate.Text="Validando...";try{var data=await LicenseGate.Request(catalog,"/api/license/activate",key.Text);State=new LicenseState{key=key.Text.Trim().ToUpperInvariant(),expiresAt=data.ContainsKey("expiresAt")?Convert.ToString(data["expiresAt"]):null,lastValidated=DateTime.UtcNow.ToString("o")};DialogResult=DialogResult.OK;Close();}catch(Exception ex){message.ForeColor=Ice.Amber;message.Text=ex.Message;}finally{activate.Enabled=true;activate.Text="Ativar e continuar";}}
}
