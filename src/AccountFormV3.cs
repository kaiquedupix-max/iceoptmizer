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
  using(var glow=new PathGradientBrush(new[]{new Point(300,-80),new Point(Width+100,-80),new Point(Width+100,Height+60),new Point(300,Height+60)})){glow.CenterPoint=new PointF(Width*.67f,Height*.34f);glow.CenterColor=Color.FromArgb(56,15,101,158);glow.SurroundColors=new[]{Color.Transparent,Color.Transparent,Color.Transparent,Color.Transparent};g.FillRectangle(glow,ClientRectangle);}
  using(var shade=new SolidBrush(Color.FromArgb(90,1,8,17)))g.FillPolygon(shade,new[]{new Point(0,0),new Point(178,0),new Point(92,250),new Point(0,350)});
  Point[] left={new Point(0,360),new Point(70,281),new Point(117,337),new Point(168,292),new Point(230,410),new Point(0,490)};using(var b=new LinearGradientBrush(new Rectangle(0,250,260,260),Color.FromArgb(185,8,45,77),Color.FromArgb(240,2,10,19),60))g.FillPolygon(b,left);
  Point[] right={new Point(Width-195,430),new Point(Width-128,337),new Point(Width-78,379),new Point(Width-28,294),new Point(Width,330),new Point(Width,Height),new Point(Width-240,Height)};using(var b=new LinearGradientBrush(new Rectangle(Width-250,280,250,240),Color.FromArgb(125,12,59,92),Color.FromArgb(245,2,11,21),120))g.FillPolygon(b,right);
  using(var p=new Pen(Color.FromArgb(28,Ice.Cyan))){for(int i=0;i<7;i++)g.DrawLine(p,Width-250+i*42,Height,Width-80+i*20,45);}
  using(var edge=new Pen(Color.FromArgb(170,34,182,236),1))using(var path=Ice.Round(new RectangleF(1,1,Width-3,Height-3),9))g.DrawPath(edge,path);
 }
}

public class AccountForm:Form {
 readonly Catalog catalog;readonly LoginScene scene=new LoginScene();readonly TextBox username=new TextBox(),password=new TextBox();readonly Label message=new Label();readonly IceButton submit=new IceButton(),create=new IceButton();readonly CheckBox remember=new CheckBox();public LicenseState State;public bool Remember{get{return remember.Checked;}}
 [DllImport("user32.dll")]static extern bool ReleaseCapture();[DllImport("user32.dll")]static extern IntPtr SendMessage(IntPtr h,int msg,IntPtr w,IntPtr l);
 public AccountForm(Catalog c,string previous){catalog=c;Text="Ice Optimizer";ClientSize=new Size(742,508);MinimumSize=MaximumSize=new Size(742,508);StartPosition=FormStartPosition.CenterScreen;FormBorderStyle=FormBorderStyle.None;BackColor=Ice.Bg;ForeColor=Ice.Text;Font=Ice.Font(9);KeyPreview=true;using(var s=System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ice.ico"))if(s!=null)Icon=new Icon(s);
  scene.Dock=DockStyle.Fill;Controls.Add(scene);scene.MouseDown+=(s,e)=>{if(e.Button==MouseButtons.Left){ReleaseCapture();SendMessage(Handle,0xA1,(IntPtr)2,IntPtr.Zero);}};
  AddWindowButton("—",635,()=>WindowState=FormWindowState.Minimized);AddWindowButton("□",674,()=>{});AddWindowButton("×",713,Close);
  var crystal=new Panel{BackColor=Color.Transparent};crystal.SetBounds(326,23,90,100);crystal.Paint+=(s,e)=>Ice.Crystal(e.Graphics,new RectangleF(13,0,65,94),Motion.Phase);scene.Controls.Add(crystal);
  var brand=Make("ICE OPTIMIZER",20,Ice.Text,true);brand.TextAlign=ContentAlignment.MiddleCenter;brand.SetBounds(230,126,282,35);scene.Controls.Add(brand);var sub=Make("DESEMPENHO REAL. SEM COMPLICAÇÃO.",7.5f,Color.FromArgb(102,184,220));sub.TextAlign=ContentAlignment.MiddleCenter;sub.SetBounds(230,160,282,18);scene.Controls.Add(sub);
  var left=Make("OTIMIZE\nJOGUE\nPRODUZA\nSEM LIMITES.",10,Color.FromArgb(48,178,235));left.SetBounds(40,128,150,95);scene.Controls.Add(left);var right=Make("SISTEMA\n+ LEVE\n+ RÁPIDO\n+ ESTÁVEL.",11,Color.FromArgb(73,211,255));right.SetBounds(620,360,100,100);scene.Controls.Add(right);
  var card=new IceSurface{Interactive=true};card.SetBounds(216,188,310,270);scene.Controls.Add(card);
  var login=new IceButton{Text="Entrar",Primary=true,Active=true};login.SetBounds(7,7,151,35);card.Controls.Add(login);var register=new IceButton{Text="Criar conta"};register.SetBounds(157,7,146,35);register.Click+=(s,e)=>OpenSite();card.Controls.Add(register);
  Style(username);username.Text=previous;var uf=new IceInput(username);uf.SetBounds(12,50,286,36);uf.Padding=new Padding(31,10,10,7);uf.Paint+=(s,e)=>Ice.Icon(e.Graphics,"Perfil",new Rectangle(10,9,17,17),username.Focused?Ice.Cyan:Ice.Muted);card.Controls.Add(uf);
  Style(password);password.UseSystemPasswordChar=true;var pf=new IceInput(password);pf.SetBounds(12,90,286,36);pf.Padding=new Padding(31,10,10,7);pf.Paint+=(s,e)=>Ice.Icon(e.Graphics,"Recuperação",new Rectangle(10,9,17,17),password.Focused?Ice.Cyan:Ice.Muted);card.Controls.Add(pf);
  remember.Text="Lembrar de mim";remember.Checked=true;remember.ForeColor=Ice.Text;remember.BackColor=Color.Transparent;remember.SetBounds(13,131,120,24);remember.Cursor=Cursors.Hand;remember.CheckedChanged+=(s,e)=>IceSound.Select();card.Controls.Add(remember);var forgot=Make("Esqueceu a senha?",8,Ice.Cyan);forgot.TextAlign=ContentAlignment.MiddleRight;forgot.SetBounds(172,132,126,22);card.Controls.Add(forgot);
  submit.Text="Entrar no Ice Optimizer   →";submit.Primary=true;submit.SetBounds(12,159,286,38);submit.Click+=async(s,e)=>await Submit();card.Controls.Add(submit);
  var divider=Make("────────────   ou   ────────────",8,Ice.Muted);divider.TextAlign=ContentAlignment.MiddleCenter;divider.SetBounds(12,200,286,22);card.Controls.Add(divider);create.Text="Criar uma nova conta";create.SetBounds(12,226,286,35);create.Click+=(s,e)=>OpenSite();card.Controls.Add(create);
  message.TextAlign=ContentAlignment.MiddleCenter;message.ForeColor=Ice.Muted;message.BackColor=Color.Transparent;message.SetBounds(195,464,352,28);scene.Controls.Add(message);var version=Make("v3.0.0",7.5f,Ice.Muted);version.SetBounds(15,477,80,20);scene.Controls.Add(version);
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
