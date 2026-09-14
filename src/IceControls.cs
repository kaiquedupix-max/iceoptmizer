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
 static Timer timer=new Timer{Interval=33};public static bool Enabled=true;public static event Action Tick;public static float Phase;
 static Motion(){timer.Tick+=(s,e)=>{if(Enabled){Phase+=.045f;if(Tick!=null)Tick();}};timer.Start();}
 public static void Set(bool value){Enabled=value;if(Tick!=null)Tick();}
}
public class IceButton:Button {
 public bool Primary,Active;public string Glyph;float hover;bool over,down;
 public IceButton(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);FlatStyle=FlatStyle.Flat;FlatAppearance.BorderSize=0;BackColor=Ice.Bg;ForeColor=Ice.Text;Cursor=Cursors.Hand;Height=42;Font=Ice.Font(10);Motion.Tick+=Animate;}
 void Animate(){float target=over?1:0;if(Math.Abs(hover-target)>.02f){hover=Motion.Enabled?hover+(target-hover)*.24f:target;Invalidate();}}
 protected override void OnMouseEnter(EventArgs e){over=true;Animate();base.OnMouseEnter(e);}protected override void OnMouseLeave(EventArgs e){over=false;down=false;Animate();base.OnMouseLeave(e);}
 protected override void OnMouseDown(MouseEventArgs e){down=true;Invalidate();base.OnMouseDown(e);}protected override void OnMouseUp(MouseEventArgs e){down=false;Invalidate();base.OnMouseUp(e);}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;g.Clear(BackColor);Color fill=Primary?Ice.Mix(Ice.Cyan,Color.FromArgb(169,243,255),hover):Ice.Mix(Active?Color.FromArgb(24,58,76):Ice.Panel,Color.FromArgb(30,65,87),hover);if(!Enabled)fill=Ice.Mix(fill,Ice.Bg,.65f);if(down)fill=Ice.Mix(fill,Ice.Blue,.25f);Ice.Box(g,new Rectangle(1,1,Width-3,Height-3),fill,Active?Ice.Cyan:Primary?fill:Ice.Line,9);Color text=Enabled?(Primary?Ice.Bg:Active?Ice.Cyan:Ice.Text):Ice.Muted;int x=14;if(!string.IsNullOrEmpty(Glyph)){Ice.Icon(g,Glyph,new Rectangle(13,(Height-19)/2,19,19),text);x=42;}Ice.TextAt(g,Text,new Rectangle(x,0,Width-x-12,Height),10,text,Primary);if(Focused)using(var pen=new Pen(Ice.Cyan){DashStyle=DashStyle.Dot})g.DrawRectangle(pen,5,5,Width-11,Height-11);}
 protected override void Dispose(bool disposing){if(disposing)Motion.Tick-=Animate;base.Dispose(disposing);}
}
public class IceSurface:Panel {
 public IceSurface(){DoubleBuffered=true;BackColor=Ice.Bg;Padding=new Padding(18);}
 protected override void OnPaint(PaintEventArgs e){e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;Ice.Box(e.Graphics,new Rectangle(0,0,Width-1,Height-1),Ice.Panel,Ice.Line,14);base.OnPaint(e);}
}
public class SnowCanvas:Panel {
 class Flake {public float x,y,speed,size,drift;}
 readonly List<Flake> flakes=new List<Flake>();readonly Random random=new Random(2202);
 public SnowCanvas(){DoubleBuffered=true;BackColor=Ice.Bg;for(int i=0;i<74;i++)flakes.Add(new Flake{x=(float)random.NextDouble(),y=(float)random.NextDouble(),speed=.0013f+(float)random.NextDouble()*.0034f,size=2.4f+(float)random.NextDouble()*4.8f,drift=(float)random.NextDouble()*6});Motion.Tick+=Fall;}
 void Fall(){if(!Visible)return;foreach(var f in flakes){f.y+=f.speed;if(f.y>1.03f){f.y=-.03f;f.x=(float)random.NextDouble();}}Invalidate();}
 protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;foreach(var f in flakes){float x=f.x*Width+(float)Math.Sin(Motion.Phase+f.drift)*13,y=f.y*Height;int alpha=Math.Min(150,(int)(35+f.size*15));using(var b=new SolidBrush(Color.FromArgb(alpha,Ice.Text)))g.FillEllipse(b,x,y,f.size,f.size);if(f.size>5)using(var p=new Pen(Color.FromArgb(alpha/2,Ice.Cyan),1)){g.DrawLine(p,x-f.size*.45f,y+f.size/2,x+f.size*1.45f,y+f.size/2);g.DrawLine(p,x+f.size/2,y-f.size*.45f,x+f.size/2,y+f.size*1.45f);}}}
 protected override void Dispose(bool d){if(d)Motion.Tick-=Fall;base.Dispose(d);}
}
public class Hero:Control {
 public Hero(){DoubleBuffered=true;BackColor=Ice.Bg;Motion.Tick+=Animate;}
 void Animate(){if(Visible)Invalidate();}
 protected override void Dispose(bool d){if(d)Motion.Tick-=Animate;base.Dispose(d);}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;using(var shape=Ice.Round(new RectangleF(0,0,Width-1,Height-1),17)){using(var grad=new LinearGradientBrush(ClientRectangle,Color.FromArgb(17,44,63),Color.FromArgb(13,26,45),10))g.FillPath(grad,shape);using(var pen=new Pen(Color.FromArgb(39,77,96)))g.DrawPath(pen,shape);}
  int artX=Width-210;using(var pen=new Pen(Color.FromArgb(20,Ice.Cyan))){for(int i=0;i<8;i++){g.DrawLine(pen,artX-90+i*48,Height,artX+50+i*30,0);}for(int y=25;y<Height;y+=32)g.DrawLine(pen,artX-20,y,Width,y);}
  if(Width>700){Ice.Crystal(g,new RectangleF(Width-185,8,158,142),Motion.Phase);Ice.Crystal(g,new RectangleF(Width-68,95,42,42),-Motion.Phase);}
  Ice.TextAt(g,"ICE OPTIMIZER   /   EDIÇÃO GLACIAL",new Rectangle(26,16,Width-52,22),8.5f,Ice.Cyan,true);
  Ice.TextAt(g,"Mais controle. Menos distrações.",new Rectangle(24,47,Width-(Width>700?215:40),43),23,Ice.Text,true);
  Ice.TextAt(g,"Escolha seus ajustes e monte um plano para o seu Windows.",new Rectangle(26,99,Width-(Width>700?218:45),25),10.5f,Ice.Muted);
  Ice.TextAt(g,"160 ações   ·   Download verificado   ·   Por Maciota",new Rectangle(26,129,Width-50,23),9,Ice.Cyan);
 }
}
public class ActionCard:Control {
 public ActionItem Item;public bool Checked;public event Action<ActionCard> Toggle,Details;bool over;float hover;
 public ActionCard(ActionItem a){Item=a;Height=218;TabStop=true;AccessibleName=a.title;AccessibleDescription=a.description+" "+a.warning;AccessibleRole=AccessibleRole.CheckButton;Cursor=Cursors.Hand;SetStyle(ControlStyles.Selectable|ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);Motion.Tick+=Animate;}
 void Animate(){if(!Visible)return;float target=over?1:0;if(Math.Abs(hover-target)>.02){hover=Motion.Enabled?hover+(target-hover)*.22f:target;Invalidate();}}
 protected override void OnMouseEnter(EventArgs e){over=true;Animate();base.OnMouseEnter(e);}protected override void OnMouseLeave(EventArgs e){over=false;Animate();base.OnMouseLeave(e);}
 public void Flip(){if(!Enabled)return;if(Toggle!=null)Toggle(this);Invalidate();AccessibilityNotifyClients(AccessibleEvents.StateChange,-1);}
 protected override void OnMouseClick(MouseEventArgs e){Focus();if(e.Y>Height-43){if(Details!=null)Details(this);}else Flip();base.OnMouseClick(e);}
 protected override void OnKeyDown(KeyEventArgs e){if(e.KeyCode==Keys.Space){Flip();e.Handled=true;}if(e.KeyCode==Keys.Enter){if(Details!=null)Details(this);e.Handled=true;}base.OnKeyDown(e);}
 protected override void OnGotFocus(EventArgs e){Invalidate();base.OnGotFocus(e);}protected override void OnLostFocus(EventArgs e){Invalidate();base.OnLostFocus(e);}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;g.Clear(Ice.Bg);var fill=Ice.Mix(Checked?Color.FromArgb(17,44,59):Ice.Panel,Color.FromArgb(22,48,68),hover);Ice.Box(g,new Rectangle(1,2,Width-4,Height-5),fill,Checked?Ice.Cyan:Ice.Mix(Ice.Line,Ice.Blue,hover*.55f),13);
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
 public IceProgress(){DoubleBuffered=true;Height=8;Motion.Tick+=Animate;}
 void Animate(){if(!Visible)return;if(Running||Math.Abs(shown-Value)>.001f){shown=Motion.Enabled?shown+(Value-shown)*.12f:Value;Invalidate();}}
 protected override void OnPaint(PaintEventArgs e){var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;Ice.Box(g,new Rectangle(0,0,Width-1,Height-1),Ice.Line,Ice.Line,4);int w=(int)((Width-1)*(Motion.Enabled?shown:Value));if(w>2)Ice.Box(g,new Rectangle(0,0,w,Height-1),Ice.Cyan,Ice.Cyan,4);if(Running){int x=(int)((Math.Sin(Motion.Phase*3)+1)*.5*Math.Max(0,Width-60));using(var b=new SolidBrush(Color.FromArgb(65,Ice.Cyan)))g.FillRectangle(b,x,0,60,Height);}}
 protected override void Dispose(bool d){if(d)Motion.Tick-=Animate;base.Dispose(d);}
}
public class ExecutionOverlay:Control {
 public float Value;public string Detail="Preparando o ambiente...";float shown;
 public ExecutionOverlay(){DoubleBuffered=true;Visible=false;Cursor=Cursors.WaitCursor;TabStop=true;Motion.Tick+=Animate;SetStyle(ControlStyles.Selectable|ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);}
 void Animate(){if(!Visible)return;shown=Motion.Enabled?shown+(Math.Max(0,Math.Min(1,Value))-shown)*.1f:Value;Invalidate();}
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
  int bx=cx+163,by=cy+208,bw=cw-210,bh=24;Ice.Box(g,new Rectangle(bx,by,bw,bh),Color.FromArgb(8,20,34),Ice.Line,12);int fill=(int)((bw-4)*p);if(fill>3){using(var path=Ice.Round(new RectangleF(bx+2,by+2,fill,bh-4),10))using(var grad=new LinearGradientBrush(new Rectangle(bx,by,Math.Max(1,bw),bh),Ice.Blue,Ice.Cyan,LinearGradientMode.Horizontal))g.FillPath(grad,path);for(int i=14;i<fill;i+=27)using(var shine=new Pen(Color.FromArgb(65,Color.White),2))g.DrawLine(shine,bx+i,by+5,bx+i+8,by+bh-6);}
  Ice.TextAt(g,Math.Round(p*100)+"%",new Rectangle(bx,by+35,bw,36),17,Ice.Cyan,true);Ice.TextAt(g,Detail,new Rectangle(bx,by+76,bw,56),10,Ice.Text,false,true);Ice.TextAt(g,"Os cliques estão bloqueados para evitar comandos acidentais. ESC solicita uma parada segura após a ação atual.",new Rectangle(cx+36,cy+365,cw-72,40),9,Ice.Muted,false,true);
 }
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
