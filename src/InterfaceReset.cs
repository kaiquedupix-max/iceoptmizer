using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
 protected override void OnMouseEnter(EventArgs e){over=true;Invalidate();base.OnMouseEnter(e);}protected override void OnMouseLeave(EventArgs e){over=down=false;Invalidate();base.OnMouseLeave(e);}protected override void OnMouseDown(MouseEventArgs e){down=true;IceAudio.Click();Invalidate();base.OnMouseDown(e);}protected override void OnMouseUp(MouseEventArgs e){down=false;Invalidate();base.OnMouseUp(e);}
 protected override void OnPaint(PaintEventArgs e){if(!over&&!down)return;using(var b=new SolidBrush(Color.FromArgb(down?38:18,105,225,255)))e.Graphics.FillRectangle(b,ClientRectangle);if(Glow)using(var p=new Pen(Color.FromArgb(150,111,232,255)))e.Graphics.DrawRectangle(p,0,0,Width-1,Height-1);}
}

sealed class RememberToggle:Control {
 public bool Checked;
 public RememberToggle(){Cursor=Cursors.Hand;SetStyle(ControlStyles.SupportsTransparentBackColor|ControlStyles.UserPaint|ControlStyles.OptimizedDoubleBuffer,true);BackColor=Color.Transparent;}
 protected override void OnMouseClick(MouseEventArgs e){Checked=!Checked;IceAudio.Click();Invalidate();base.OnMouseClick(e);}
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

sealed class AccountCanvas:Panel {
 static Image artwork;public AccountCanvas(){DoubleBuffered=true;SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);}
 protected override void OnPaint(PaintEventArgs e){if(artwork==null){using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("account-reference.jpg"))using(var source=Image.FromStream(stream))artwork=new Bitmap(source);}e.Graphics.InterpolationMode=System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;e.Graphics.DrawImage(artwork,ClientRectangle);DrawAccount(e.Graphics);IceEffects.DrawSnow(e.Graphics,Width,Height);}
 void DrawAccount(Graphics g){var state=LicenseGate.Current;if(state==null)return;string user=string.IsNullOrWhiteSpace(state.username)?"usuário":state.username,plan=LicenseGate.PlanName(state.plan),remaining=LicenseGate.Remaining(state);DateTime value;string expiry=DateTime.TryParse(state.expiresAt,out value)?value.ToLocalTime().ToString("dd/MM/yyyy HH:mm"):"Permanente",seen=DateTime.TryParse(state.lastValidated,out value)?value.ToLocalTime().ToString("dd/MM/yyyy HH:mm"):"Agora";
  CoverText(g,user,new Rectangle(557,18,59,18),8.5f,true,Color.FromArgb(5,17,30),Color.White);CoverText(g,"Plano "+plan,new Rectangle(557,39,67,15),7,false,Color.FromArgb(4,16,28),Color.FromArgb(150,181,205));
  CoverText(g,user,new Rectangle(210,119,105,20),11.5f,true,Color.FromArgb(8,22,39),Color.White);CoverText(g,"Plano "+plan,new Rectangle(210,141,110,16),7.5f,false,Color.FromArgb(8,23,40),Color.FromArgb(174,200,217));
  CoverText(g,remaining,new Rectangle(164,175,132,18),8.5f,true,Color.FromArgb(7,22,41),Color.FromArgb(57,212,255));CoverText(g,"Válida até "+expiry,new Rectangle(164,208,178,17),7.2f,false,Color.FromArgb(7,22,38),Color.FromArgb(191,215,229));
  CoverText(g,user,new Rectangle(588,138,108,18),7.8f,false,Color.FromArgb(7,23,39),Color.FromArgb(202,224,237));CoverText(g,plan,new Rectangle(588,162,108,18),7.8f,false,Color.FromArgb(6,22,38),Color.FromArgb(202,224,237));CoverText(g,"Ativa",new Rectangle(588,186,108,18),7.8f,false,Color.FromArgb(8,22,39),Color.FromArgb(70,224,174));CoverText(g,expiry,new Rectangle(588,210,122,18),7.8f,false,Color.FromArgb(8,23,39),Color.FromArgb(202,224,237));CoverText(g,"Protegido",new Rectangle(588,234,108,18),7.8f,false,Color.FromArgb(7,22,38),Color.White);CoverText(g,seen,new Rectangle(588,258,122,18),7.8f,false,Color.FromArgb(7,22,38),Color.FromArgb(202,224,237));
 }
 void CoverText(Graphics g,string text,Rectangle r,float size,bool bold,Color back,Color fore){float sx=Width/744f,sy=Height/444f;var scaled=new Rectangle((int)Math.Round(r.X*sx),(int)Math.Round(r.Y*sy),(int)Math.Ceiling(r.Width*sx),(int)Math.Ceiling(r.Height*sy));using(var b=new SolidBrush(back))g.FillRectangle(b,scaled);using(var font=new Font("Segoe UI",size*Math.Min(sx,sy),bold?FontStyle.Bold:FontStyle.Regular))TextRenderer.DrawText(g,text,font,scaled,fore,TextFormatFlags.Left|TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis|TextFormatFlags.NoPadding);}
}

sealed class HomeCanvas:Panel {
 static Image artwork;
 public HomeCanvas(){DoubleBuffered=true;SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);}
 protected override void OnPaint(PaintEventArgs e){
  if(artwork==null){using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("home-reference.png"))using(var source=Image.FromStream(stream))artwork=new Bitmap(source);}
  e.Graphics.InterpolationMode=System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
  e.Graphics.DrawImage(artwork,ClientRectangle);
  IceEffects.DrawSnow(e.Graphics,Width,Height);
 }
}

// The interface is rebuilt one page at a time from the approved references.
public sealed class MainWindow:Form {
 readonly Catalog catalog=Catalog.Load();readonly HomeCanvas home=new HomeCanvas();readonly AccountCanvas account=new AccountCanvas();readonly Dictionary<string,IcePageCanvas> pages=new Dictionary<string,IcePageCanvas>();readonly Timer transition=new Timer{Interval=16},ambient=new Timer{Interval=16};Control current,outgoing,incoming;int transitionFrame;
 [DllImport("user32.dll")]static extern bool ReleaseCapture();[DllImport("user32.dll")]static extern IntPtr SendMessage(IntPtr h,int msg,IntPtr w,IntPtr l);
 public MainWindow(){Text="Ice Optimizer";StartPosition=FormStartPosition.CenterScreen;FormBorderStyle=FormBorderStyle.None;BackColor=Color.FromArgb(3,13,24);DoubleBuffered=true;ClientSize=new Size(1168,705);MinimumSize=MaximumSize=Size;using(var stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("ice.ico"))if(stream!=null)Icon=new Icon(stream);
  transition.Tick+=AnimateTransition;ambient.Tick+=(s,e)=>{if(WindowState!=FormWindowState.Minimized&&current!=null&&!transition.Enabled)current.Invalidate();};ambient.Start();
  home.MouseDown+=DragWindow;account.MouseDown+=DragWindow;
  AddHit(home,1028,5,43,34,()=>WindowState=FormWindowState.Minimized);AddHit(home,1071,5,43,34,()=>{});AddHit(home,1114,5,50,34,Close);
  foreach(string page in new[]{"Windows","Jogos","Hardware","Reparos","Aplicativos","Ativações","Recuperação","Configurações"}){pages[page]=new IcePageCanvas(catalog,page,Navigate,ExecuteItems);pages[page].MouseDown+=DragWindow;}
  AddHit(home,11,85,174,41,ShowHome);AddHit(home,14,130,164,39,()=>ShowPage("Windows"));AddHit(home,14,175,164,39,()=>ShowPage("Jogos"));AddHit(home,14,220,164,39,()=>ShowPage("Hardware"));AddHit(home,14,265,164,39,()=>ShowPage("Reparos"));AddHit(home,14,310,164,39,()=>ShowPage("Aplicativos"));AddHit(home,14,355,164,39,()=>ShowPage("Recuperação"));AddHit(home,14,507,164,42,ShowAccount);AddHit(home,14,553,164,42,()=>ShowPage("Configurações"));
  AddHit(home,228,388,180,31,()=>ShowPage("Windows"));AddHit(home,450,388,179,31,()=>ShowPage("Jogos"));AddHit(home,670,388,180,31,()=>ShowPage("Hardware"));
  AddHit(home,228,577,180,31,()=>ShowPage("Reparos"));AddHit(home,450,577,179,31,()=>ShowPage("Aplicativos"));AddHit(home,670,577,180,31,()=>ShowPage("Recuperação"));
  AddHit(home,889,552,248,51,()=>ShowPage("Windows"));
  AddScaledHit(account,628,4,35,28,()=>WindowState=FormWindowState.Minimized);AddScaledHit(account,664,4,37,28,()=>{});AddScaledHit(account,701,4,41,28,Close);
  AddScaledHit(account,9,48,118,36,ShowHome);AddScaledHit(account,9,84,118,30,()=>ShowPage("Windows"));AddScaledHit(account,9,114,118,30,()=>ShowPage("Jogos"));AddScaledHit(account,9,144,118,30,()=>ShowPage("Hardware"));AddScaledHit(account,9,174,118,30,()=>ShowPage("Reparos"));AddScaledHit(account,9,204,118,30,()=>ShowPage("Aplicativos"));AddScaledHit(account,9,234,118,30,()=>ShowPage("Recuperação"));AddScaledHit(account,9,352,118,33,()=>ShowPage("Configurações"));AddScaledHit(account,165,325,135,35,OpenAccount);AddScaledHit(account,304,325,133,35,Logout);AddScaledHit(account,478,367,236,32,OpenPlans);
  ShowHome();
 }
 void DragWindow(object sender,MouseEventArgs e){if(e.Button==MouseButtons.Left&&e.Y<58){ReleaseCapture();SendMessage(Handle,0xA1,(IntPtr)2,IntPtr.Zero);}}
 void AddHit(Control surface,int x,int y,int w,int h,Action action){var hit=new HitArea{Glow=true};hit.SetBounds(x,y,w,h);hit.Click+=(s,e)=>action();surface.Controls.Add(hit);hit.BringToFront();}
 void AddScaledHit(Control surface,int x,int y,int w,int h,Action action){AddHit(surface,(int)Math.Round(x*1168d/744d),(int)Math.Round(y*705d/444d),(int)Math.Round(w*1168d/744d),(int)Math.Round(h*705d/444d),action);}
 void Switch(Control page,string title){if(page==current||transition.Enabled)return;Text=title;if(current==null){current=page;page.Dock=DockStyle.Fill;Controls.Add(page);page.Invalidate();return;}if(!IceEffects.AnimationsEnabled){Controls.Remove(current);current=page;page.Dock=DockStyle.Fill;Controls.Add(page);page.BringToFront();page.Invalidate();return;}outgoing=current;incoming=page;current=page;outgoing.Dock=DockStyle.None;outgoing.Bounds=new Rectangle(0,0,ClientSize.Width,ClientSize.Height);incoming.Dock=DockStyle.None;incoming.Bounds=new Rectangle(ClientSize.Width,0,ClientSize.Width,ClientSize.Height);incoming.Enabled=outgoing.Enabled=false;Controls.Add(incoming);incoming.BringToFront();transitionFrame=0;transition.Start();}
 void AnimateTransition(object sender,EventArgs e){transitionFrame++;double p=Math.Min(1,transitionFrame/15d),ease=1-Math.Pow(1-p,3);incoming.Left=(int)Math.Round(ClientSize.Width*(1-ease));outgoing.Left=(int)Math.Round(-ClientSize.Width*.14*ease);incoming.Invalidate();outgoing.Invalidate();if(p>=1){transition.Stop();Controls.Remove(outgoing);outgoing.Left=0;outgoing.Enabled=true;incoming.Dock=DockStyle.Fill;incoming.Enabled=true;incoming.Focus();incoming=null;outgoing=null;}}
 void ShowHome(){Switch(home,"Ice Optimizer — Início");}
 void ShowAccount(){Switch(account,"Ice Optimizer — Minha conta");}
 void ShowPage(string name){IcePageCanvas page;if(pages.TryGetValue(name,out page))Switch(page,"Ice Optimizer — "+name);}
 void Navigate(string name){if(name=="home")ShowHome();else if(name=="account")ShowAccount();else ShowPage(name);}
 async void ExecuteItems(ActionItem[] items){
  var problems=BatchPlan.Problems(items);if(problems.Count>0){MessageBox.Show(string.Join("\n",problems),"Revise sua seleção",MessageBoxButtons.OK,MessageBoxIcon.Warning);return;}
  if(MessageBox.Show("Você selecionou "+items.Length+" ação(ões). O mouse ficará bloqueado durante a execução. Deseja continuar?","Revisar plano",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
  var progress=new IceProgressForm();Enabled=false;UseWaitCursor=true;progress.Show(this);try{
   string root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"ice optimizer","runs");
   var results=await BatchPlan.Run(items,a=>Payload.Prepare(catalog,a,root,LicenseGate.Current==null?null:LicenseGate.Current.token,x=>{}),(a,path)=>Payload.Execute(path,x=>{}),()=>false,(phase,index,total,item)=>progress.Report(phase,index,total,item),true);
   progress.Complete();await Task.Delay(550);int failed=results.Count(x=>x.code!=0);MessageBox.Show(failed==0?"As ações selecionadas foram concluídas.":failed+" ação(ões) terminaram com erro. Revise o resultado antes de continuar.","Ice Optimizer",MessageBoxButtons.OK,failed==0?MessageBoxIcon.Information:MessageBoxIcon.Warning);
  }catch(Exception ex){MessageBox.Show(ex.Message,"Não foi possível concluir",MessageBoxButtons.OK,MessageBoxIcon.Error);}finally{progress.Close();progress.Dispose();Enabled=true;UseWaitCursor=false;Activate();}
 }
 void OpenAccount(){try{Process.Start(new ProcessStartInfo("https://ice-optimizer-web-production.up.railway.app/"){UseShellExecute=true});}catch{}}
 void OpenPlans(){try{Process.Start(new ProcessStartInfo("https://ice-optimizer-web-production.up.railway.app/#planos"){UseShellExecute=true});}catch{}}
 void Logout(){LicenseGate.Logout();Close();}
 public void UiTest(){if(catalog.actions==null||catalog.actions.Length<100)throw new Exception("Catálogo indisponível.");var resources=Assembly.GetExecutingAssembly().GetManifestResourceNames();if(!resources.Contains("ice-click.wav")||!resources.Contains("home-reference.png")||!resources.Contains("account-reference.jpg"))throw new Exception("Recursos visuais ou sonoros ausentes.");if(pages.Count!=8)throw new Exception("Páginas incompletas.");}
 public void Render(string file,string mode=""){ambient.Stop();LicenseGate.UsePreview();if(current!=null){Controls.Remove(current);current=null;}if(mode=="profile")ShowAccount();else if(mode=="hardware")ShowPage("Hardware");else if(mode=="games")ShowPage("Jogos");else if(mode=="selected"||mode=="windows")ShowPage("Windows");else if(mode=="progress"||mode=="repairs")ShowPage("Reparos");else if(mode=="apps")ShowPage("Aplicativos");else if(mode=="activations")ShowPage("Ativações");else if(mode=="recovery")ShowPage("Recuperação");else if(mode=="settings")ShowPage("Configurações");else ShowHome();current.Invalidate();ShowInTaskbar=false;Show();Application.DoEvents();using(var bmp=new Bitmap(Width,Height)){DrawToBitmap(bmp,new Rectangle(0,0,Width,Height));bmp.Save(file);}Hide();}
}
