using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public static class Ice {
 public static readonly Color Bg=Color.FromArgb(8,15,26),Side=Color.FromArgb(11,21,35),Panel=Color.FromArgb(15,29,46),Line=Color.FromArgb(32,54,73),Text=Color.FromArgb(231,244,252),Muted=Color.FromArgb(137,162,181),Cyan=Color.FromArgb(112,230,246),Blue=Color.FromArgb(80,153,232),Amber=Color.FromArgb(245,190,114);
 static Dictionary<string,Font> fonts=new Dictionary<string,Font>();
 public static Font Font(float size,bool bold=false){string key=size+"/"+bold;if(!fonts.ContainsKey(key))fonts[key]=new Font("Segoe UI",size,bold?FontStyle.Bold:FontStyle.Regular);return fonts[key];}
 public static Color Mix(Color a,Color b,float t){t=Math.Max(0,Math.Min(1,t));return Color.FromArgb((int)(a.R+(b.R-a.R)*t),(int)(a.G+(b.G-a.G)*t),(int)(a.B+(b.B-a.B)*t));}
 public static GraphicsPath Round(RectangleF r,float radius=12){float d=Math.Min(radius*2,Math.Min(r.Width,r.Height));var p=new GraphicsPath();p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;}
 public static void Box(Graphics g,Rectangle r,Color fill,Color line,int radius=12){if(r.Width<2||r.Height<2)return;using(var p=Round(r,radius)){using(var b=new SolidBrush(fill))g.FillPath(b,p);using(var pen=new Pen(line))g.DrawPath(pen,p);}}
 public static void TextAt(Graphics g,string text,Rectangle r,float size,Color color,bool bold=false,bool wrap=false){TextRenderer.DrawText(g,text??"",Font(size,bold),r,color,TextFormatFlags.NoPadding|TextFormatFlags.NoPrefix|(wrap?TextFormatFlags.WordBreak:TextFormatFlags.EndEllipsis|TextFormatFlags.VerticalCenter));}
 public static void Crystal(Graphics g,RectangleF r,float phase=0){
  var state=g.Save();g.TranslateTransform(r.X+r.Width/2,r.Y+r.Height/2+(float)Math.Sin(phase)*2);g.ScaleTransform(r.Width/100,r.Height/100);g.SmoothingMode=SmoothingMode.AntiAlias;
  PointF top=new PointF(0,-45),left=new PointF(-32,-12),bottom=new PointF(0,45),right=new PointF(32,-12),center=new PointF(0,0);
  using(var glow=new SolidBrush(Color.FromArgb(15,Ice.Cyan)))g.FillEllipse(glow,-46,-46,92,92);
  using(var b=new SolidBrush(Color.FromArgb(195,248,255)))g.FillPolygon(b,new[]{top,left,center});
  using(var b=new SolidBrush(Color.FromArgb(84,211,240)))g.FillPolygon(b,new[]{top,center,right});
  using(var b=new SolidBrush(Color.FromArgb(40,111,171)))g.FillPolygon(b,new[]{left,bottom,center});
  using(var b=new SolidBrush(Color.FromArgb(130,223,247)))g.FillPolygon(b,new[]{right,center,bottom});
  using(var pen=new Pen(Color.FromArgb(210,Ice.Cyan),1.1f)){g.DrawPolygon(pen,new[]{top,left,bottom,right});g.DrawLine(pen,left,right);g.DrawLine(pen,top,bottom);}
  using(var p=new Pen(Color.FromArgb(75,Ice.Cyan),1)){g.DrawLine(p,-45,3,-36,3);g.DrawLine(p,36,3,45,3);g.DrawLine(p,0,-55,0,-49);g.DrawLine(p,0,49,0,55);}
  g.Restore(state);
 }
 public static void Icon(Graphics g,string name,Rectangle r,Color c){
  var state=g.Save();g.TranslateTransform(r.X,r.Y);g.ScaleTransform(r.Width/24f,r.Height/24f);g.SmoothingMode=SmoothingMode.AntiAlias;
  using(var p=new Pen(c,1.6f)){p.StartCap=p.EndCap=LineCap.Round;p.LineJoin=LineJoin.Round;
   if(name=="Jogos"){g.DrawPolygon(p,new[]{new PointF(5,7),new PointF(19,7),new PointF(22,17),new PointF(18,19),new PointF(15,15),new PointF(9,15),new PointF(6,19),new PointF(2,17)});g.DrawLine(p,6,10,6,14);g.DrawLine(p,4,12,8,12);g.DrawEllipse(p,16,10,1,1);g.DrawEllipse(p,19,12,1,1);}
   else if(name=="Hardware"){g.DrawRectangle(p,6,6,12,12);g.DrawRectangle(p,9,9,6,6);for(int i=8;i<18;i+=4){g.DrawLine(p,i,3,i,6);g.DrawLine(p,i,18,i,21);g.DrawLine(p,3,i,6,i);g.DrawLine(p,18,i,21,i);}}
   else if(name=="Recuperação"){g.DrawPolygon(p,new[]{new Point(12,2),new Point(21,6),new Point(19,16),new Point(12,22),new Point(5,16),new Point(3,6)});g.DrawLine(p,8,12,11,15);g.DrawLine(p,11,15,16,9);}
   else if(name=="Reparos"){g.DrawArc(p,11,2,10,10,40,275);g.DrawLine(p,13,11,3,19);g.DrawLine(p,3,19,5,22);g.DrawLine(p,5,22,16,12);}
   else if(name=="Histórico"){g.DrawEllipse(p,3,3,18,18);g.DrawLine(p,12,6,12,12);g.DrawLine(p,12,12,16,14);}
   else if(name=="Perfil"){g.DrawEllipse(p,8,3,8,8);g.DrawArc(p,4,12,16,10,180,180);}
   else if(name=="Configurações"){for(int i=5;i<21;i+=7){g.DrawLine(p,3,i,21,i);g.DrawEllipse(p,i-2,i-2,4,4);}}
   else if(name=="search"){g.DrawEllipse(p,3,3,12,12);g.DrawLine(p,14,14,21,21);}
   else if(name=="check"){g.DrawLine(p,5,12,10,17);g.DrawLine(p,10,17,20,6);}
   else if(name=="play"){g.DrawPolygon(p,new[]{new Point(7,4),new Point(20,12),new Point(7,20)});}
   else if(name=="arrow"){g.DrawLine(p,5,12,19,12);g.DrawLine(p,14,7,19,12);g.DrawLine(p,14,17,19,12);}
   else{g.DrawRectangle(p,3,3,7,7);g.DrawRectangle(p,14,3,7,7);g.DrawRectangle(p,3,14,7,7);g.DrawRectangle(p,14,14,7,7);}
  }g.Restore(state);
 }
}
public static class Motion {
 [DllImport("winmm.dll")]static extern uint timeBeginPeriod(uint period);
 [DllImport("winmm.dll")]static extern uint timeEndPeriod(uint period);
 static Timer timer=new Timer{Interval=15};static System.Diagnostics.Stopwatch clock=System.Diagnostics.Stopwatch.StartNew();static long previous;
 public static bool Enabled=true;public static event Action Tick;public static float Phase;public static float Delta=1f/60f;
 static Motion(){timeBeginPeriod(1);previous=clock.ElapsedTicks;timer.Tick+=(s,e)=>{if(!Enabled)return;long now=clock.ElapsedTicks;Delta=Math.Max(.001f,Math.Min(.05f,(float)(now-previous)/System.Diagnostics.Stopwatch.Frequency));previous=now;Phase+=Delta*1.5f;if(Tick!=null)Tick();};timer.Start();Application.ApplicationExit+=(s,e)=>timeEndPeriod(1);}
 public static float Follow(float value,float target,float speed){return Enabled?target+(value-target)*(float)Math.Exp(-speed*Delta):target;}
 public static void Set(bool value){Enabled=value;if(Tick!=null)Tick();}
}
public class SlideTransition:Control {
 readonly Bitmap from,to;readonly int direction;float progress;
 public SlideTransition(Bitmap oldFrame,Bitmap newFrame,int dir){from=oldFrame;to=newFrame;direction=dir<0?-1:1;DoubleBuffered=true;BackColor=Ice.Panel;SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.ResizeRedraw,true);}
 public void SetProgress(float value){progress=Math.Max(0,Math.Min(1,value));Invalidate();Update();}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.Clear(BackColor);g.InterpolationMode=InterpolationMode.HighQualityBilinear;float ease=1-(float)Math.Pow(1-progress,4);int oldX=(int)(-direction*Width*ease),newX=(int)(direction*Width*(1-ease));if(from!=null)g.DrawImageUnscaled(from,oldX,0);if(to!=null)g.DrawImageUnscaled(to,newX,0);}
 protected override void Dispose(bool disposing){if(disposing){if(from!=null)from.Dispose();if(to!=null)to.Dispose();}base.Dispose(disposing);}
 public static Bitmap Snapshot(Control control){var image=new Bitmap(Math.Max(1,control.Width),Math.Max(1,control.Height));control.DrawToBitmap(image,new Rectangle(0,0,image.Width,image.Height));return image;}
}
public class IceButton:Button {
 public bool Primary,Active;public string Glyph;float hover,press;bool over,down;Point pointer;
 public IceButton(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.SupportsTransparentBackColor,true);FlatStyle=FlatStyle.Flat;FlatAppearance.BorderSize=0;BackColor=Color.Transparent;ForeColor=Ice.Text;Cursor=Cursors.Hand;Height=42;Font=Ice.Font(10);Motion.Tick+=Animate;}
 void Animate(){float h=Motion.Follow(hover,over?1:0,18),p=Motion.Follow(press,down?1:0,25);if(Math.Abs(h-hover)>.002f||Math.Abs(p-press)>.002f){hover=h;press=p;Invalidate();}}
 protected override void OnMouseEnter(EventArgs e){over=true;Animate();base.OnMouseEnter(e);}protected override void OnMouseLeave(EventArgs e){over=false;down=false;Animate();base.OnMouseLeave(e);}
 protected override void OnMouseMove(MouseEventArgs e){pointer=e.Location;if(over)Invalidate();base.OnMouseMove(e);}
 protected override void OnMouseDown(MouseEventArgs e){down=true;Invalidate();base.OnMouseDown(e);}protected override void OnMouseUp(MouseEventArgs e){down=false;Invalidate();base.OnMouseUp(e);}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;if(BackColor.A==0)base.OnPaintBackground(e);else g.Clear(BackColor);int lift=(int)Math.Round(hover*1.5f-press*2);var body=new Rectangle(3,3-lift,Width-7,Height-7);Color fill=Primary?Ice.Mix(Ice.Cyan,Color.FromArgb(195,250,255),hover*.72f):Ice.Mix(Active?Color.FromArgb(22,60,81):Color.FromArgb(14,31,48),Color.FromArgb(27,67,89),hover);if(!Enabled)fill=Ice.Mix(fill,Ice.Bg,.65f);if(down)fill=Ice.Mix(fill,Ice.Blue,.22f);if((over||Active)&&Enabled){using(var halo=new SolidBrush(Color.FromArgb((int)(20+hover*32),Ice.Cyan)))using(var hp=Ice.Round(new RectangleF(0,1,Width-1,Height-2),14))g.FillPath(halo,hp);}Ice.Box(g,body,fill,Active?Ice.Cyan:Primary?Color.FromArgb(190,Ice.Cyan):Ice.Mix(Ice.Line,Ice.Cyan,hover*.68f),12);if(over&&Enabled){int radius=Math.Max(42,Width/3);using(var path=new GraphicsPath()){path.AddEllipse(pointer.X-radius,pointer.Y-radius,radius*2,radius*2);using(var brush=new PathGradientBrush(path)){brush.CenterPoint=pointer;brush.CenterColor=Color.FromArgb(Primary?62:40,Color.White);brush.SurroundColors=new[]{Color.FromArgb(0,Ice.Cyan)};var state=g.Save();using(var clip=Ice.Round(body,12)){g.SetClip(clip);g.FillPath(brush,path);}g.Restore(state);}}}if(Active&&!Primary)using(var accent=new SolidBrush(Ice.Cyan))using(var gp=Ice.Round(new RectangleF(6,Height/2-10-lift,3,20),2))g.FillPath(accent,gp);Color text=Enabled?(Primary?Ice.Bg:Active?Ice.Cyan:Ice.Text):Ice.Muted;int x=15;if(!string.IsNullOrEmpty(Glyph)){Ice.Icon(g,Glyph,new Rectangle(14,(Height-19)/2-lift,19,19),text);x=43;}Ice.TextAt(g,Text,new Rectangle(x,-lift,Width-x-12,Height),10,text,Primary);if(Focused)using(var pen=new Pen(Color.FromArgb(190,Ice.Cyan)){DashStyle=DashStyle.Dot})using(var gp=Ice.Round(new RectangleF(7,7-lift,Width-15,Height-15),8))g.DrawPath(pen,gp);}
 protected override void Dispose(bool disposing){if(disposing)Motion.Tick-=Animate;base.Dispose(disposing);}
}
public class IceSurface:Panel {
 public bool Interactive;bool over;float hover;Point pointer;
 public IceSurface(){DoubleBuffered=true;BackColor=Ice.Bg;Padding=new Padding(18);Motion.Tick+=Animate;}
 void Animate(){float next=Motion.Follow(hover,Interactive&&over?1:0,15);if(Math.Abs(next-hover)>.002f){hover=next;Invalidate();}}
 protected override void OnMouseEnter(EventArgs e){over=true;Animate();base.OnMouseEnter(e);}protected override void OnMouseLeave(EventArgs e){over=false;Animate();base.OnMouseLeave(e);}protected override void OnMouseMove(MouseEventArgs e){pointer=e.Location;if(Interactive)Invalidate();base.OnMouseMove(e);}
 protected override void OnPaint(PaintEventArgs e){e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;var body=new Rectangle(1,2-(int)Math.Round(hover*2),Width-3,Height-4);Ice.Box(e.Graphics,body,Ice.Mix(Ice.Panel,Color.FromArgb(19,43,61),hover*.65f),Ice.Mix(Ice.Line,Ice.Cyan,hover*.6f),15);if(Interactive&&over){int radius=Math.Max(120,Width/3);using(var path=new GraphicsPath()){path.AddEllipse(pointer.X-radius,pointer.Y-radius,radius*2,radius*2);using(var brush=new PathGradientBrush(path)){brush.CenterPoint=pointer;brush.CenterColor=Color.FromArgb((int)(24*hover),Ice.Cyan);brush.SurroundColors=new[]{Color.FromArgb(0,Ice.Cyan)};var state=e.Graphics.Save();using(var clip=Ice.Round(body,15)){e.Graphics.SetClip(clip);e.Graphics.FillPath(brush,path);}e.Graphics.Restore(state);}}}base.OnPaint(e);}
 protected override void Dispose(bool d){if(d)Motion.Tick-=Animate;base.Dispose(d);}
}
public class SnowCanvas:Panel {
 class Flake {public float x,y,speed,size,drift;}
 readonly List<Flake> flakes=new List<Flake>();readonly Random random=new Random(2202);
 public SnowCanvas(){DoubleBuffered=true;BackColor=Ice.Bg;for(int i=0;i<74;i++)flakes.Add(new Flake{x=(float)random.NextDouble(),y=(float)random.NextDouble(),speed=.00065f+(float)random.NextDouble()*.0017f,size=2.4f+(float)random.NextDouble()*4.8f,drift=(float)random.NextDouble()*6});Motion.Tick+=Fall;}
 void Fall(){if(!Visible)return;float frame=Motion.Delta*60f;foreach(var f in flakes){f.y+=f.speed*frame;if(f.y>1.03f){f.y=-.03f;f.x=(float)random.NextDouble();}}Invalidate();}
 protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;foreach(var f in flakes){float x=f.x*Width+(float)Math.Sin(Motion.Phase+f.drift)*13,y=f.y*Height;int alpha=Math.Min(150,(int)(35+f.size*15));using(var b=new SolidBrush(Color.FromArgb(alpha,Ice.Text)))g.FillEllipse(b,x,y,f.size,f.size);if(f.size>5)using(var p=new Pen(Color.FromArgb(alpha/2,Ice.Cyan),1)){g.DrawLine(p,x-f.size*.45f,y+f.size/2,x+f.size*1.45f,y+f.size/2);g.DrawLine(p,x+f.size/2,y-f.size*.45f,x+f.size/2,y+f.size*1.45f);}}}
 protected override void Dispose(bool d){if(d)Motion.Tick-=Fall;base.Dispose(d);}
}
public class Hero:Control {
 public Hero(){DoubleBuffered=true;BackColor=Ice.Bg;Motion.Tick+=Animate;}
 void Animate(){if(Visible)Invalidate();}
 protected override void Dispose(bool d){if(d)Motion.Tick-=Animate;base.Dispose(d);}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;using(var shape=Ice.Round(new RectangleF(0,0,Width-1,Height-1),22)){using(var grad=new LinearGradientBrush(ClientRectangle,Color.FromArgb(18,52,73),Color.FromArgb(8,20,35),12))g.FillPath(grad,shape);using(var pen=new Pen(Color.FromArgb(50,104,128)))g.DrawPath(pen,shape);}
  using(var glow=new SolidBrush(Color.FromArgb(24,Ice.Cyan)))g.FillEllipse(glow,Width-265,-115,330,330);int artX=Width-225;using(var pen=new Pen(Color.FromArgb(22,Ice.Cyan))){for(int i=0;i<8;i++)g.DrawLine(pen,artX-70+i*42,Height,artX+40+i*30,0);for(int y=20;y<Height;y+=30)g.DrawLine(pen,artX-15,y,Width,y);}
  if(Width>720){Ice.Crystal(g,new RectangleF(Width-177,5,145,139),Motion.Phase);Ice.Box(g,new Rectangle(Width-218,148,184,28),Color.FromArgb(16,40,56),Color.FromArgb(53,104,126),14);Ice.TextAt(g,"●  SISTEMA PROTEGIDO",new Rectangle(Width-203,148,158,28),8,Ice.Cyan,true);}
  Ice.TextAt(g,"ICE OPTIMIZER   /   PAINEL GLACIAL",new Rectangle(27,18,Width-55,22),8.5f,Ice.Cyan,true);
  Ice.TextAt(g,"Seu Windows, na temperatura certa.",new Rectangle(25,48,Width-(Width>720?235:45),43),22,Ice.Text,true);
  Ice.TextAt(g,"Escolha com clareza, revise cada impacto e acompanhe tudo em tempo real.",new Rectangle(27,94,Width-(Width>720?245:45),25),10,Ice.Muted);
  string[] badges={"160 AJUSTES ORGANIZADOS","HWID PROTEGIDO","EXECUÇÃO VERIFICADA"};int x=27;foreach(var badge in badges){int w=badge.StartsWith("160")?180:badge.StartsWith("HWID")?143:176;Ice.Box(g,new Rectangle(x,140,w,32),Color.FromArgb(12,31,47),Color.FromArgb(38,78,99),9);Ice.TextAt(g,badge,new Rectangle(x+12,140,w-20,32),7.5f,Ice.Muted,true);x+=w+9;}
 }
}
public class ActionCard:Control {
 public ActionItem Item;public bool Checked;public event Action<ActionCard> Toggle,Details;bool over;float hover;Point pointer;
 public ActionCard(ActionItem a){Item=a;Height=218;TabStop=true;AccessibleName=a.title;AccessibleDescription=a.description+" "+a.warning;AccessibleRole=AccessibleRole.CheckButton;Cursor=Cursors.Hand;SetStyle(ControlStyles.Selectable|ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);Motion.Tick+=Animate;}
 void Animate(){if(!Visible)return;float next=Motion.Follow(hover,over?1:0,16);if(Math.Abs(next-hover)>.002){hover=next;Invalidate();}}
 protected override void OnMouseEnter(EventArgs e){over=true;Animate();base.OnMouseEnter(e);}protected override void OnMouseLeave(EventArgs e){over=false;Animate();base.OnMouseLeave(e);}
 protected override void OnMouseMove(MouseEventArgs e){pointer=e.Location;if(over)Invalidate();base.OnMouseMove(e);}
 public void Flip(){if(!Enabled)return;if(Toggle!=null)Toggle(this);Invalidate();AccessibilityNotifyClients(AccessibleEvents.StateChange,-1);}
 protected override void OnMouseClick(MouseEventArgs e){Focus();if(e.Y>Height-43){if(Details!=null)Details(this);}else Flip();base.OnMouseClick(e);}
 protected override void OnKeyDown(KeyEventArgs e){if(e.KeyCode==Keys.Space){Flip();e.Handled=true;}if(e.KeyCode==Keys.Enter){if(Details!=null)Details(this);e.Handled=true;}base.OnKeyDown(e);}
 protected override void OnGotFocus(EventArgs e){Invalidate();base.OnGotFocus(e);}protected override void OnLostFocus(EventArgs e){Invalidate();base.OnLostFocus(e);}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;g.Clear(Ice.Bg);int lift=(int)Math.Round(hover*3);var body=new Rectangle(2,5-lift,Width-5,Height-8);var fill=Ice.Mix(Checked?Color.FromArgb(17,44,59):Ice.Panel,Color.FromArgb(23,53,72),hover*.8f);if(over)using(var halo=new SolidBrush(Color.FromArgb((int)(30*hover),Ice.Cyan)))using(var hp=Ice.Round(new RectangleF(0,2-lift,Width-1,Height-4),16))g.FillPath(halo,hp);Ice.Box(g,body,fill,Checked?Ice.Cyan:Ice.Mix(Ice.Line,Ice.Cyan,hover*.7f),15);if(over){int radius=150;using(var path=new GraphicsPath()){path.AddEllipse(pointer.X-radius,pointer.Y-radius,radius*2,radius*2);using(var brush=new PathGradientBrush(path)){brush.CenterPoint=pointer;brush.CenterColor=Color.FromArgb((int)(34*hover),Ice.Cyan);brush.SurroundColors=new[]{Color.FromArgb(0,Ice.Cyan)};var state=g.Save();using(var clip=Ice.Round(body,15)){g.SetClip(clip);g.FillPath(brush,path);}g.Restore(state);}}}
  Ice.Box(g,new Rectangle(17,18,34,34),Color.FromArgb(23,50,67),Color.FromArgb(29,62,80),8);Ice.Icon(g,Item.category,new Rectangle(24,25,20,20),Ice.Cyan);
  Ice.TextAt(g,Item.category.ToUpperInvariant(),new Rectangle(62,17,Width-117,19),8,Ice.Muted,true);
  Ice.TextAt(g,Item.risk,new Rectangle(62,36,Width-117,19),8,Item.risk=="Alto impacto"?Ice.Amber:Ice.Cyan);
  Ice.Box(g,new Rectangle(Width-42,23,22,22),Checked?Ice.Cyan:Ice.Bg,Checked?Ice.Cyan:Ice.Muted,6);if(Checked)Ice.Icon(g,"check",new Rectangle(Width-40,25,18,18),Ice.Bg);
  Ice.TextAt(g,Item.title,new Rectangle(18,66,Width-36,44),12,Ice.Text,true,true);
  Ice.TextAt(g,Item.description,new Rectangle(18,113,Width-36,55),9.3f,Ice.Muted,false,true);
  using(var p=new Pen(Ice.Line))g.DrawLine(p,18,Height-43,Width-18,Height-43);
  Ice.TextAt(g,"Ver detalhes",new Rectangle(18,Height-37,Width-62,28),9,Ice.Cyan);Ice.Icon(g,"arrow",new Rectangle(Width-39,Height-31,16,16),Ice.Cyan);
  if(Focused)using(var pen=new Pen(Ice.Cyan){DashStyle=DashStyle.Dot})g.DrawRectangle(pen,7,8,Width-16,Height-17);
 }
 protected override AccessibleObject CreateAccessibilityInstance(){return new CardAccess(this);}
 class CardAccess:ControlAccessibleObject {ActionCard card;public CardAccess(ActionCard c):base(c){card=c;}public override AccessibleStates State{get{return base.State|(card.Checked?AccessibleStates.Checked:0);}}public override string DefaultAction{get{return "Marcar ou desmarcar";}}public override void DoDefaultAction(){card.Flip();}}
 protected override void Dispose(bool d){if(d)Motion.Tick-=Animate;base.Dispose(d);}
}
public class IceProgress:Control {
 public float Value;public bool Running;float shown;
 public IceProgress(){DoubleBuffered=true;Height=12;Motion.Tick+=Animate;SetStyle(ControlStyles.ResizeRedraw,true);}
 void Animate(){if(!Visible)return;if(Running||Math.Abs(shown-Value)>.001f){shown=Motion.Follow(shown,Value,9);Invalidate();}}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;var track=new Rectangle(0,1,Width-1,Height-3);Ice.Box(g,track,Color.FromArgb(7,18,31),Color.FromArgb(39,72,92),Math.Max(4,Height/2));int w=(int)((Width-3)*(Motion.Enabled?shown:Value));if(w>3){using(var path=Ice.Round(new RectangleF(2,3,w,Math.Max(3,Height-7)),Math.Max(3,Height/2-2)))using(var grad=new LinearGradientBrush(new Rectangle(0,0,Math.Max(1,Width),Height),Color.FromArgb(62,151,228),Color.FromArgb(154,245,255),LinearGradientMode.Horizontal))g.FillPath(grad,path);using(var shine=new Pen(Color.FromArgb(150,Color.White),1))g.DrawLine(shine,5,4,Math.Max(5,w-2),4);}if(Running){int span=Math.Min(150,Math.Max(50,Width/6)),x=(int)((Motion.Phase*95)%(Width+span))-span;using(var path=new GraphicsPath()){path.AddRectangle(new Rectangle(x,2,span,Math.Max(1,Height-4)));using(var grad=new LinearGradientBrush(new Rectangle(x,0,Math.Max(1,span),Height),Color.FromArgb(0,Color.White),Color.FromArgb(115,Color.White),LinearGradientMode.Horizontal)){var state=g.Save();using(var clip=Ice.Round(track,Height/2)){g.SetClip(clip);g.FillPath(grad,path);}g.Restore(state);}}}}
 protected override void Dispose(bool d){if(d)Motion.Tick-=Animate;base.Dispose(d);}
}
public class ExecutionOverlay:Control {
 public float Value;public string Detail="Preparando o ambiente...";float shown;
 public ExecutionOverlay(){DoubleBuffered=true;Visible=false;Cursor=Cursors.WaitCursor;TabStop=true;Motion.Tick+=Animate;SetStyle(ControlStyles.Selectable|ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);}
 void Animate(){if(!Visible)return;shown=Motion.Follow(shown,Math.Max(0,Math.Min(1,Value)),8);Invalidate();}
 protected override void OnMouseDown(MouseEventArgs e){Focus();}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;using(var veil=new SolidBrush(Color.FromArgb(242,Ice.Bg)))g.FillRectangle(veil,ClientRectangle);
  int cw=Math.Min(720,Math.Max(520,Width-80)),ch=430,cx=(Width-cw)/2,cy=(Height-ch)/2;Ice.Box(g,new Rectangle(cx,cy,cw,ch),Color.FromArgb(18,36,55),Color.FromArgb(55,112,140),22);
  Ice.TextAt(g,"OTIMIZAÇÃO EM ANDAMENTO",new Rectangle(cx+36,cy+30,cw-72,24),9,Ice.Cyan,true);Ice.TextAt(g,"Deixe o gelo trabalhar.",new Rectangle(cx+35,cy+63,cw-70,48),25,Ice.Text,true);
  Ice.TextAt(g,"Não use o mouse dentro do Ice Optimizer até a ação atual terminar.",new Rectangle(cx+36,cy+112,cw-72,42),10,Ice.Muted,false,true);
  float p=Math.Max(0,Math.Min(1,shown));int cube=82-(int)(p*38),cubeX=cx+48,cubeY=cy+184+(82-cube);using(var glow=new SolidBrush(Color.FromArgb(22,Ice.Cyan)))g.FillEllipse(glow,cubeX-18,cubeY-20,118,118);
  PointF[] top={new PointF(cubeX,cubeY+16),new PointF(cubeX+cube*.72f,cubeY),new PointF(cubeX+cube,cubeY+18),new PointF(cubeX+cube*.28f,cubeY+34)};using(var b=new SolidBrush(Color.FromArgb(180,230,250)))g.FillPolygon(b,top);
  PointF[] left={top[0],top[3],new PointF(cubeX+cube*.28f,cubeY+cube),new PointF(cubeX,cubeY+cube-17)};using(var b=new SolidBrush(Color.FromArgb(52,145,205)))g.FillPolygon(b,left);
  PointF[] right={top[3],top[2],new PointF(cubeX+cube,cubeY+cube-18),new PointF(cubeX+cube*.28f,cubeY+cube)};using(var b=new SolidBrush(Color.FromArgb(92,210,239)))g.FillPolygon(b,right);
  using(var pool=new SolidBrush(Color.FromArgb(75+(int)(p*90),Ice.Cyan)))g.FillEllipse(pool,cubeX-10-(int)(p*18),cy+278,cube+25+(int)(p*50),8+(int)(p*8));
  int bx=cx+163,by=cy+208,bw=cw-210,bh=28;Ice.Box(g,new Rectangle(bx,by,bw,bh),Color.FromArgb(6,17,30),Color.FromArgb(48,85,105),14);int fill=(int)((bw-4)*p);if(fill>3){using(var path=Ice.Round(new RectangleF(bx+2,by+2,fill,bh-4),12))using(var grad=new LinearGradientBrush(new Rectangle(bx,by,Math.Max(1,bw),bh),Color.FromArgb(51,139,224),Color.FromArgb(143,244,255),LinearGradientMode.Horizontal))g.FillPath(grad,path);int sweep=(int)((Motion.Phase*110)%(bw+90))-70;var state=g.Save();using(var clip=Ice.Round(new RectangleF(bx+2,by+2,Math.Max(1,fill),bh-4),12)){g.SetClip(clip);using(var shine=new LinearGradientBrush(new Rectangle(bx+sweep,by,70,bh),Color.FromArgb(0,Color.White),Color.FromArgb(125,Color.White),LinearGradientMode.Horizontal))g.FillRectangle(shine,bx+sweep,by,70,bh);}g.Restore(state);}
  Ice.TextAt(g,Math.Round(p*100)+"%",new Rectangle(bx,by+35,bw,36),17,Ice.Cyan,true);Ice.TextAt(g,Detail,new Rectangle(bx,by+76,bw,56),10,Ice.Text,false,true);Ice.TextAt(g,"Os cliques estão bloqueados para evitar comandos acidentais. ESC solicita uma parada segura após a ação atual.",new Rectangle(cx+36,cy+365,cw-72,40),9,Ice.Muted,false,true);
 }
 protected override void Dispose(bool d){if(d)Motion.Tick-=Animate;base.Dispose(d);}
}
public class IceInput:Panel {
 public readonly TextBox Editor;bool focused,over,pressed;float glow;Point pointer;
 public IceInput(TextBox editor){Editor=editor;DoubleBuffered=true;SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.ResizeRedraw|ControlStyles.SupportsTransparentBackColor,true);Padding=new Padding(15,13,15,8);BackColor=Color.Transparent;Cursor=Cursors.IBeam;Editor.BorderStyle=BorderStyle.None;Editor.BackColor=Color.FromArgb(17,39,58);Editor.ForeColor=Ice.Text;Editor.Dock=DockStyle.Fill;Controls.Add(Editor);Editor.GotFocus+=(s,e)=>{focused=true;Editor.BackColor=Color.FromArgb(19,45,65);Animate();};Editor.LostFocus+=(s,e)=>{focused=false;pressed=false;Editor.BackColor=Color.FromArgb(17,39,58);Animate();};Editor.MouseEnter+=(s,e)=>{over=true;Animate();};Editor.MouseLeave+=(s,e)=>{over=false;Animate();};Editor.MouseMove+=(s,e)=>{pointer=new Point(Editor.Left+((MouseEventArgs)e).X,Editor.Top+((MouseEventArgs)e).Y);Invalidate();};Editor.MouseDown+=(s,e)=>{pressed=true;Invalidate();};Editor.MouseUp+=(s,e)=>{pressed=false;Invalidate();};Motion.Tick+=Animate;UpdateShape();}
 void UpdateShape(){if(Width>2&&Height>2){var old=Region;using(var p=Ice.Round(new RectangleF(0,0,Width,Height),12))Region=new Region(p);if(old!=null)old.Dispose();}}
 void Animate(){float target=focused?1:over?.62f:0;float next=Motion.Follow(glow,target,18);if(Math.Abs(next-glow)>.002f){glow=next;Invalidate();}}
 protected override void OnMouseEnter(EventArgs e){over=true;Animate();base.OnMouseEnter(e);}protected override void OnMouseLeave(EventArgs e){over=false;Animate();base.OnMouseLeave(e);}protected override void OnMouseDown(MouseEventArgs e){pressed=true;Editor.Focus();Invalidate();base.OnMouseDown(e);}protected override void OnMouseUp(MouseEventArgs e){pressed=false;Invalidate();base.OnMouseUp(e);}
 protected override void OnSizeChanged(EventArgs e){base.OnSizeChanged(e);UpdateShape();}
 protected override void OnPaintBackground(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;g.Clear(Ice.Mix(Color.FromArgb(35,70,91),Ice.Cyan,.18f+glow*.72f));Color fill=pressed?Color.FromArgb(20,47,67):focused?Color.FromArgb(19,45,65):Color.FromArgb(17,39,58);using(var p=Ice.Round(new RectangleF(2,2,Width-4,Height-4),10))using(var b=new SolidBrush(fill))g.FillPath(b,p);if((over||focused)&&pointer!=Point.Empty){int radius=95;using(var p=new GraphicsPath()){p.AddEllipse(pointer.X-radius,pointer.Y-radius,radius*2,radius*2);using(var b=new PathGradientBrush(p)){b.CenterPoint=pointer;b.CenterColor=Color.FromArgb((int)(35+glow*30),Ice.Cyan);b.SurroundColors=new[]{Color.FromArgb(0,Ice.Cyan)};var state=g.Save();using(var clip=Ice.Round(new RectangleF(2,2,Width-4,Height-4),10)){g.SetClip(clip);g.FillPath(b,p);}g.Restore(state);}}}}
 protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;using(var p=new Pen(Ice.Mix(Color.FromArgb(62,101,121),Ice.Cyan,.18f+glow*.82f),focused?1.8f:1.2f))using(var gp=Ice.Round(new RectangleF(2.5f,2.5f,Width-6,Height-6),9))e.Graphics.DrawPath(p,gp);}
 protected override void Dispose(bool d){if(d)Motion.Tick-=Animate;base.Dispose(d);}
}
public class SearchField:TextBox {
 [DllImport("user32.dll",CharSet=CharSet.Unicode,EntryPoint="SendMessageW")]static extern IntPtr Cue(IntPtr h,int msg,IntPtr w,string text);
 protected override void OnHandleCreated(EventArgs e){base.OnHandleCreated(e);Cue(Handle,0x1501,(IntPtr)1,"Buscar ações...  Ctrl+F");}
}
public class CardDeck:Panel {
 int offset,total;bool arranging,dragging;int dragY,dragStart;
 public CardDeck(){DoubleBuffered=true;AutoScroll=false;SetStyle(ControlStyles.ResizeRedraw,true);}
 public void LayoutCards(){if(arranging)return;arranging=true;try{int usable=Math.Max(250,ClientSize.Width-20);int columns=usable>=690?2:1;int width=(usable-(columns-1)*14)/columns;total=((Controls.Count+columns-1)/columns)*232;offset=Math.Max(0,Math.Min(offset,Math.Max(0,total-Height)));int i=0;foreach(Control c in Controls){c.SetBounds((i%columns)*(width+14),(i/columns)*232-offset,width,218);i++;}Invalidate();}finally{arranging=false;}}
 public void ResetScroll(){offset=0;LayoutCards();}
 protected override void OnSizeChanged(EventArgs e){base.OnSizeChanged(e);LayoutCards();}
 protected override void OnMouseWheel(MouseEventArgs e){offset-=Math.Sign(e.Delta)*88;LayoutCards();base.OnMouseWheel(e);}
 protected override void OnControlAdded(ControlEventArgs e){base.OnControlAdded(e);e.Control.GotFocus+=(s,a)=>{var c=(Control)s;if(c.Top<0){offset+=c.Top;LayoutCards();}else if(c.Bottom>Height){offset+=c.Bottom-Height;LayoutCards();}};}
 protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);if(total<=Height)return;int h=Math.Max(32,Height*Height/total),y=offset*(Height-h)/Math.Max(1,total-Height);Ice.Box(e.Graphics,new Rectangle(Width-8,y,4,h),Ice.Mix(Ice.Line,Ice.Cyan,dragging?.7f:.25f),Ice.Line,2);}
 protected override void OnMouseDown(MouseEventArgs e){if(e.X>=Width-18&&total>Height){dragging=true;Capture=true;dragY=e.Y;dragStart=offset;}base.OnMouseDown(e);}
 protected override void OnMouseMove(MouseEventArgs e){if(dragging){int thumb=Math.Max(32,Height*Height/total);offset=dragStart+(e.Y-dragY)*Math.Max(1,total-Height)/Math.Max(1,Height-thumb);LayoutCards();}base.OnMouseMove(e);}
 protected override void OnMouseUp(MouseEventArgs e){dragging=false;Capture=false;Invalidate();base.OnMouseUp(e);}
}
