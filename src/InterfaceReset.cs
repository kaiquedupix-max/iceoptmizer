using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

// Clean interface boundary. The previous visual system was intentionally removed.
// Keep business logic out of this file while the new UI is designed from scratch.
public sealed class AccountForm : Form {
 readonly Catalog catalog;
 readonly TextBox login=new TextBox(),password=new TextBox();
 readonly Label message=new Label();
 readonly Button submit=new Button(),create=new Button();
 readonly CheckBox remember=new CheckBox();
 public LicenseState State;
 public bool Remember{get{return remember.Checked;}}

 public AccountForm(Catalog value,string previous){
  catalog=value;Text="Ice Optimizer — nova interface";ClientSize=new Size(640,420);StartPosition=FormStartPosition.CenterScreen;BackColor=Color.FromArgb(12,17,24);ForeColor=Color.White;Font=new Font("Segoe UI",10);FormBorderStyle=FormBorderStyle.FixedSingle;MaximizeBox=false;
  var title=new Label{Text="NOVA INTERFACE EM CONSTRUÇÃO",Font=new Font("Segoe UI",16,FontStyle.Bold),ForeColor=Color.White,AutoSize=false,TextAlign=ContentAlignment.MiddleCenter};title.SetBounds(40,35,560,45);Controls.Add(title);
  var note=new Label{Text="A camada visual anterior foi removida. Este formulário mantém somente o acesso enquanto a nova experiência é criada.",ForeColor=Color.Silver,AutoSize=false,TextAlign=ContentAlignment.MiddleCenter};note.SetBounds(70,84,500,48);Controls.Add(note);
  login.Text=previous;login.SetBounds(145,155,350,34);password.UseSystemPasswordChar=true;password.SetBounds(145,205,350,34);Controls.Add(login);Controls.Add(password);
  remember.Text="Lembrar de mim";remember.Checked=true;remember.SetBounds(145,250,180,28);Controls.Add(remember);
  submit.Text="Entrar";submit.SetBounds(355,300,140,40);submit.Click+=async(s,e)=>await Submit();Controls.Add(submit);
  create.Text="Criar conta no site";create.SetBounds(145,300,190,40);create.Click+=(s,e)=>System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://ice-optimizer-web-production.up.railway.app/"){UseShellExecute=true});Controls.Add(create);
  message.AutoSize=false;message.TextAlign=ContentAlignment.MiddleCenter;message.ForeColor=Color.Silver;message.SetBounds(80,355,480,42);Controls.Add(message);AcceptButton=submit;
 }
 async Task Submit(){
  if(login.Text.Trim().Length<3||password.Text.Length<8){message.Text="Informe usuário ou e-mail e uma senha válida.";return;}
  submit.Enabled=create.Enabled=false;message.Text="Validando conta...";
  try{var data=await LicenseGate.Request(catalog,"/api/account/login",new Dictionary<string,string>{{"login",login.Text.Trim()},{"password",password.Text}});State=new LicenseState{token=Convert.ToString(data["token"]),username=Convert.ToString(data["username"]),plan=Convert.ToString(data["plan"]),expiresAt=data.ContainsKey("expiresAt")?Convert.ToString(data["expiresAt"]):null,createdAt=data.ContainsKey("createdAt")?Convert.ToString(data["createdAt"]):null,lastValidated=DateTime.UtcNow.ToString("o")};DialogResult=DialogResult.OK;Close();}
  catch(Exception ex){message.Text=ex.Message;}
  finally{submit.Enabled=create.Enabled=true;}
 }
}

public sealed class MainWindow : Form {
 readonly Catalog catalog=Catalog.Load();
 public MainWindow(){
  Text="Ice Optimizer — reconstrução da interface";ClientSize=new Size(1100,720);MinimumSize=new Size(900,600);StartPosition=FormStartPosition.CenterScreen;BackColor=Color.FromArgb(12,17,24);ForeColor=Color.White;Font=new Font("Segoe UI",10);
  var title=new Label{Text="BASE VISUAL LIMPA",Font=new Font("Segoe UI",24,FontStyle.Bold),AutoSize=false,TextAlign=ContentAlignment.MiddleCenter};title.SetBounds(100,235,900,60);Controls.Add(title);
  var note=new Label{Text="A interface anterior foi removida por completo.\nO catálogo, a execução das ações, contas, HWID e integração com o site continuam preservados para a reconstrução.",ForeColor=Color.Silver,AutoSize=false,TextAlign=ContentAlignment.MiddleCenter};note.SetBounds(170,310,760,90);Controls.Add(note);
 }
 public void UiTest(){if(catalog.actions==null||catalog.actions.Length<100)throw new Exception("Catálogo indisponível na base limpa.");}
 public void Render(string file,string mode=""){ShowInTaskbar=false;Show();Application.DoEvents();using(var bmp=new Bitmap(Width,Height)){DrawToBitmap(bmp,new Rectangle(0,0,Width,Height));bmp.Save(file);}Hide();}
}
