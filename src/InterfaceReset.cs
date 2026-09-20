using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

sealed class LoginCanvas:Panel {
 static Image artwork;
 public LoginCanvas(){DoubleBuffered=true;SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);}
 protected override void OnPaint(PaintEventArgs e){if(artwork==null){using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("login-reference.jpg"))using(var source=Image.FromStream(stream))artwork=new Bitmap(source);}e.Graphics.DrawImage(artwork,ClientRectangle);}
}

sealed class HitArea:Control {
 bool over,down;public bool Glow;
 public HitArea(){Cursor=Cursors.Hand;SetStyle(ControlStyles.SupportsTransparentBackColor|ControlStyles.UserPaint|ControlStyles.OptimizedDoubleBuffer,true);BackColor=Color.Transparent;}
 protected override void OnMouseEnter(EventArgs e){over=true;Invalidate();base.OnMouseEnter(e);}protected override void OnMouseLeave(EventArgs e){over=down=false;Invalidate();base.OnMouseLeave(e);}protected override void OnMouseDown(MouseEventArgs e){down=true;Invalidate();base.OnMouseDown(e);}protected override void OnMouseUp(MouseEventArgs e){down=false;Invalidate();base.OnMouseUp(e);}
 protected override void OnPaint(PaintEventArgs e){if(!over&&!down)return;using(var b=new SolidBrush(Color.FromArgb(down?38:18,105,225,255)))e.Graphics.FillRectangle(b,ClientRectangle);if(Glow)using(var p=new Pen(Color.FromArgb(150,111,232,255)))e.Graphics.DrawRectangle(p,0,0,Width-1,Height-1);}
}

sealed class RememberToggle:Control {
 public bool Checked;
 public RememberToggle(){Cursor=Cursors.Hand;SetStyle(ControlStyles.SupportsTransparentBackColor|ControlStyles.UserPaint|ControlStyles.OptimizedDoubleBuffer,true);BackColor=Color.Transparent;}
 protected override void OnMouseClick(MouseEventArgs e){Checked=!Checked;Invalidate();base.OnMouseClick(e);}
 protected override void OnPaint(PaintEventArgs e){if(!Checked)return;using(var b=new SolidBrush(Color.FromArgb(42,158,224)))e.Graphics.FillRectangle(b,1,1,11,11);using(var p=new Pen(Color.White,1.5f)){e.Graphics.DrawLine(p,3,6,5,9);e.Graphics.DrawLine(p,5,9,10,3);}}
}

public sealed class AccountForm:Form {
 readonly Catalog catalog;readonly LoginCanvas canvas=new LoginCanvas();readonly TextBox login=new TextBox(),password=new TextBox();readonly Label message=new Label();readonly HitArea submit=new HitArea{Glow=true},create=new HitArea(),register=new HitArea(),forgot=new HitArea();readonly RememberToggle remember=new RememberToggle();public LicenseState State;public bool Remember{get{return remember.Checked;}}
 [DllImport("user32.dll",CharSet=CharSet.Unicode,EntryPoint="SendMessageW")]static extern IntPtr Cue(IntPtr h,int msg,IntPtr w,string text);
 [DllImport("user32.dll")]static extern bool ReleaseCapture();[DllImport("user32.dll")]static extern IntPtr SendMessage(IntPtr h,int msg,IntPtr w,IntPtr l);
 public AccountForm(Catalog value,string previous){
  catalog=value;Text="Ice Optimizer";ClientSize=new Size(743,513);MinimumSize=MaximumSize=new Size(743,513);StartPosition=FormStartPosition.CenterScreen;FormBorderStyle=FormBorderStyle.None;BackColor=Color.FromArgb(3,15,28);KeyPreview=true;
  using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("ice.ico"))if(stream!=null)Icon=new Icon(stream);
  canvas.Dock=DockStyle.Fill;Controls.Add(canvas);canvas.MouseDown+=(s,e)=>{if(e.Button==MouseButtons.Left){ReleaseCapture();SendMessage(Handle,0xA1,(IntPtr)2,IntPtr.Zero);}};
  AddHit(628,5,36,29,()=>WindowState=FormWindowState.Minimized);AddHit(664,5,37,29,()=>{});AddHit(701,5,40,29,Close);
  ConfigureInput(login,260,249,248,18,"Nome de usuário",false);login.Text=previous;ConfigureInput(password,260,288,226,18,"Senha",true);
  remember.SetBounds(231,324,14,14);canvas.Controls.Add(remember);
  register.SetBounds(375,201,149,28);register.Click+=(s,e)=>OpenSite();canvas.Controls.Add(register);
  forgot.SetBounds(438,321,82,22);forgot.Click+=(s,e)=>OpenSite();canvas.Controls.Add(forgot);
  submit.SetBounds(231,352,289,34);submit.Click+=async(s,e)=>await Submit();canvas.Controls.Add(submit);
  create.SetBounds(231,421,289,30);create.Click+=(s,e)=>OpenSite();canvas.Controls.Add(create);
  message.BackColor=Color.Transparent;message.ForeColor=Color.FromArgb(255,190,120);message.Font=new Font("Segoe UI",7.5f);message.TextAlign=ContentAlignment.MiddleCenter;message.SetBounds(218,456,314,18);canvas.Controls.Add(message);
  var accept=new HiddenAcceptButton(async()=>await Submit());Controls.Add(accept);AcceptButton=accept;KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Escape)Close();};Shown+=(s,e)=>{if(string.IsNullOrEmpty(login.Text))login.Focus();else password.Focus();};
 }
 void AddHit(int x,int y,int w,int h,Action action){var hit=new HitArea();hit.SetBounds(x,y,w,h);hit.Click+=(s,e)=>action();canvas.Controls.Add(hit);hit.BringToFront();}
 void ConfigureInput(TextBox field,int x,int y,int w,int h,string placeholder,bool secret){field.BorderStyle=BorderStyle.None;field.BackColor=Color.FromArgb(5,24,40);field.ForeColor=Color.FromArgb(216,238,250);field.Font=new Font("Segoe UI",8);field.UseSystemPasswordChar=secret;field.SetBounds(x,y,w,h);field.HandleCreated+=(s,e)=>Cue(field.Handle,0x1501,(IntPtr)1,placeholder);canvas.Controls.Add(field);field.BringToFront();}
 void OpenSite(){try{Process.Start(new ProcessStartInfo("https://ice-optimizer-web-production.up.railway.app/"){UseShellExecute=true});}catch{message.Text="Não foi possível abrir o site.";}}
 async Task Submit(){if(login.Text.Trim().Length<3||password.Text.Length<8){message.Text="Informe usuário ou e-mail e uma senha válida.";return;}submit.Enabled=create.Enabled=register.Enabled=false;message.Text="Validando conta...";try{var data=await LicenseGate.Request(catalog,"/api/account/login",new Dictionary<string,string>{{"login",login.Text.Trim()},{"password",password.Text}});State=new LicenseState{token=Convert.ToString(data["token"]),username=Convert.ToString(data["username"]),plan=Convert.ToString(data["plan"]),expiresAt=data.ContainsKey("expiresAt")?Convert.ToString(data["expiresAt"]):null,createdAt=data.ContainsKey("createdAt")?Convert.ToString(data["createdAt"]):null,lastValidated=DateTime.UtcNow.ToString("o")};DialogResult=DialogResult.OK;Close();}catch(Exception ex){message.Text=ex.Message;}finally{submit.Enabled=create.Enabled=register.Enabled=true;}}
 sealed class HiddenAcceptButton:Button {readonly Func<Task> run;public HiddenAcceptButton(Func<Task> action){run=action;SetBounds(-100,-100,1,1);TabStop=false;}protected override async void OnClick(EventArgs e){await run();base.OnClick(e);}}
}

// Remaining pages stay intentionally blank until they are rebuilt one at a time.
public sealed class MainWindow:Form {
 readonly Catalog catalog=Catalog.Load();
 public MainWindow(){Text="Ice Optimizer — reconstrução";ClientSize=new Size(1100,720);StartPosition=FormStartPosition.CenterScreen;BackColor=Color.FromArgb(5,14,24);}
 public void UiTest(){if(catalog.actions==null||catalog.actions.Length<100)throw new Exception("Catálogo indisponível.");}
 public void Render(string file,string mode=""){ShowInTaskbar=false;Show();Application.DoEvents();using(var bmp=new Bitmap(Width,Height)){DrawToBitmap(bmp,new Rectangle(0,0,Width,Height));bmp.Save(file);}Hide();}
}
