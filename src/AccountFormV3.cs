using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

public class LoginScene:SnowCanvas {
 public LoginScene(){DoubleBuffered=true;}
 protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;
  Ice.DrawCover(g,Ice.Scene,ClientRectangle);using(var shade=new SolidBrush(Color.FromArgb(72,0,8,18)))g.FillRectangle(shade,ClientRectangle);
  using(var glow=new PathGradientBrush(new[]{new Point(180,20),new Point(Width-180,20),new Point(Width-120,Height-20),new Point(120,Height-20)})){glow.CenterPoint=new PointF(Width*.5f,Height*.46f);glow.CenterColor=Color.FromArgb(18,27,137,220);glow.SurroundColors=new[]{Color.Transparent,Color.Transparent,Color.Transparent,Color.Transparent};g.FillRectangle(glow,ClientRectangle);}
  using(var edge=new Pen(Color.FromArgb(170,34,182,236),1))using(var path=Ice.Round(new RectangleF(1,1,Width-3,Height-3),9))g.DrawPath(edge,path);
 }
}

public class AccountForm:Form {
 readonly Catalog catalog;readonly LoginScene scene=new LoginScene();readonly TextBox username=new TextBox(),password=new TextBox();readonly Label message=new Label();readonly IceButton submit=new IceButton(),create=new IceButton();readonly CheckBox remember=new CheckBox();public LicenseState State;public bool Remember{get{return remember.Checked;}}
 [DllImport("user32.dll")]static extern bool ReleaseCapture();[DllImport("user32.dll")]static extern IntPtr SendMessage(IntPtr h,int msg,IntPtr w,IntPtr l);
 public AccountForm(Catalog c,string previous){catalog=c;Text="Ice Optimizer";ClientSize=new Size(900,620);MinimumSize=MaximumSize=new Size(900,620);StartPosition=FormStartPosition.CenterScreen;FormBorderStyle=FormBorderStyle.None;BackColor=Ice.Bg;ForeColor=Ice.Text;Font=Ice.Font(9);KeyPreview=true;using(var s=System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ice.ico"))if(s!=null)Icon=new Icon(s);
  scene.Dock=DockStyle.Fill;Controls.Add(scene);scene.MouseDown+=(s,e)=>{if(e.Button==MouseButtons.Left){ReleaseCapture();SendMessage(Handle,0xA1,(IntPtr)2,IntPtr.Zero);}};
  AddWindowButton("—",793,()=>WindowState=FormWindowState.Minimized);AddWindowButton("□",832,()=>{});AddWindowButton("×",871,Close);
  var crystal=new Panel{BackColor=Color.Transparent};crystal.SetBounds(405,25,90,108);crystal.Paint+=(s,e)=>Ice.Crystal(e.Graphics,new RectangleF(10,0,70,101),Motion.Phase);scene.Controls.Add(crystal);
  var brand=Make("ICE OPTIMIZER",24,Ice.Text,true);brand.TextAlign=ContentAlignment.MiddleCenter;brand.SetBounds(278,132,344,43);scene.Controls.Add(brand);var sub=Make("DESEMPENHO REAL. SEM COMPLICAÇÃO.",8.5f,Color.FromArgb(102,184,220));sub.TextAlign=ContentAlignment.MiddleCenter;sub.SetBounds(278,174,344,20);scene.Controls.Add(sub);
  var left=Make("OTIMIZE\nJOGUE\nPRODUZA\nSEM LIMITES.",11,Color.FromArgb(48,178,235));left.SetBounds(48,146,160,110);scene.Controls.Add(left);var right=Make("SISTEMA\n+ LEVE\n+ RÁPIDO\n+ ESTÁVEL.",12,Color.FromArgb(73,211,255));right.SetBounds(752,430,120,110);scene.Controls.Add(right);
  var card=new IceSurface{Interactive=true};card.SetBounds(280,210,340,310);scene.Controls.Add(card);
  var login=new IceButton{Text="Entrar",Primary=true,Active=true};login.SetBounds(7,7,166,39);card.Controls.Add(login);var register=new IceButton{Text="Criar conta"};register.SetBounds(171,7,162,39);register.Click+=(s,e)=>OpenSite();card.Controls.Add(register);
  Style(username);username.Text=previous;var uf=new IceInput(username);uf.SetBounds(14,56,312,40);uf.Padding=new Padding(33,11,10,8);uf.Paint+=(s,e)=>Ice.Icon(e.Graphics,"Perfil",new Rectangle(11,10,18,18),username.Focused?Ice.Cyan:Ice.Muted);card.Controls.Add(uf);
  Style(password);password.UseSystemPasswordChar=true;var pf=new IceInput(password);pf.SetBounds(14,103,312,40);pf.Padding=new Padding(33,11,10,8);pf.Paint+=(s,e)=>Ice.Icon(e.Graphics,"Recuperação",new Rectangle(11,10,18,18),password.Focused?Ice.Cyan:Ice.Muted);card.Controls.Add(pf);
  remember.Text="Lembrar de mim";remember.Checked=true;remember.ForeColor=Ice.Text;remember.BackColor=Color.Transparent;remember.SetBounds(15,150,128,25);remember.Cursor=Cursors.Hand;remember.CheckedChanged+=(s,e)=>IceSound.Select();card.Controls.Add(remember);var forgot=Make("Esqueceu a senha?",8,Ice.Cyan);forgot.TextAlign=ContentAlignment.MiddleRight;forgot.SetBounds(190,151,136,23);card.Controls.Add(forgot);
  submit.Text="Entrar no Ice Optimizer   →";submit.Primary=true;submit.SetBounds(14,183,312,43);submit.Click+=async(s,e)=>await Submit();card.Controls.Add(submit);
  var divider=Make("────────────   ou   ────────────",8,Ice.Muted);divider.TextAlign=ContentAlignment.MiddleCenter;divider.SetBounds(14,231,312,24);card.Controls.Add(divider);create.Text="Criar uma nova conta";create.SetBounds(14,260,312,39);create.Click+=(s,e)=>OpenSite();card.Controls.Add(create);
  message.TextAlign=ContentAlignment.MiddleCenter;message.ForeColor=Ice.Muted;message.BackColor=Color.Transparent;message.SetBounds(255,533,390,30);scene.Controls.Add(message);var version=Make("v3.1.0",8,Ice.Muted);version.SetBounds(17,583,80,22);scene.Controls.Add(version);
  AcceptButton=submit;KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Escape)Close();};Shown+=(s,e)=>username.Focus();Motion.Tick+=Pulse;
 }
 void AddWindowButton(string text,int x,Action action){var b=new IceButton{Text=text};b.SetBounds(x,6,30,25);b.Click+=(s,e)=>action();scene.Controls.Add(b);b.BringToFront();}
 Label Make(string text,float size,Color color,bool bold=false){return new Label{Text=text,Font=Ice.Font(size,bold),ForeColor=color,BackColor=Color.Transparent};}
 void Style(TextBox field){field.Font=Ice.Font(9);field.BackColor=Color.FromArgb(17,39,58);field.ForeColor=Ice.Text;field.BorderStyle=BorderStyle.None;}
 void Pulse(){if(Visible)scene.Invalidate();}
 void OpenSite(){try{Process.Start(new ProcessStartInfo("https://ice-optimizer-web-production.up.railway.app/#planos"){UseShellExecute=true});message.ForeColor=Ice.Cyan;message.Text="Crie sua conta no site e depois volte para entrar.";}catch{message.ForeColor=Ice.Amber;message.Text="Acesse o site do Ice Optimizer para criar sua conta.";}}
 async Task Submit(){if(username.Text.Trim().Length<3||password.Text.Length<8){message.ForeColor=Ice.Amber;message.Text="Informe seu usuário ou e-mail e uma senha válida.";return;}submit.Enabled=create.Enabled=false;submit.Text="Validando conta e computador...";try{var data=await LicenseGate.Request(catalog,"/api/account/login",new Dictionary<string,string>{{"login",username.Text.Trim()},{"password",password.Text}});State=new LicenseState{token=Convert.ToString(data["token"]),username=Convert.ToString(data["username"]),plan=Convert.ToString(data["plan"]),expiresAt=data.ContainsKey("expiresAt")?Convert.ToString(data["expiresAt"]):null,createdAt=data.ContainsKey("createdAt")?Convert.ToString(data["createdAt"]):null,lastValidated=DateTime.UtcNow.ToString("o")};DialogResult=DialogResult.OK;Close();}catch(Exception ex){message.ForeColor=Ice.Amber;message.Text=ex.Message;}finally{submit.Enabled=create.Enabled=true;submit.Text="Entrar no Ice Optimizer   →";}}
 protected override void Dispose(bool d){if(d)Motion.Tick-=Pulse;base.Dispose(d);}
}
