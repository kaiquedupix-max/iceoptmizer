using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

public class ActionItem {
 public string id, title, category, warning, file;
 public string[] dependencies;
 public override string ToString(){return title;}
}
public class FileInfoEntry { public string sha256; public long size; }
public class Catalog {
 public string name, author, version, repository, @ref;
 public ActionItem[] actions;
 public Dictionary<string,FileInfoEntry> files;
 public static Catalog Load(){using(var s=Assembly.GetExecutingAssembly().GetManifestResourceStream("catalog.json"))using(var r=new StreamReader(s))return new JavaScriptSerializer().Deserialize<Catalog>(r.ReadToEnd());}
}
public static class Payload {
 public static string Hash(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();}
 public static void Verify(byte[] bytes, FileInfoEntry entry){if(bytes.LongLength!=entry.size||Hash(bytes)!=entry.sha256)throw new InvalidDataException("Integridade inválida. O arquivo não será executado. Atualize o aplicativo com o responsável.");}
 public static string SafeRoot(string root){
  if(string.IsNullOrWhiteSpace(root)||root.IndexOfAny(new[]{'"','%','!','\r','\n','&','^','|','<','>'})>=0)throw new ArgumentException("Escolha uma pasta local sem caracteres especiais de comando.");
  root=Path.GetFullPath(root);if(root.StartsWith(@"\\"))throw new ArgumentException("Escolha uma pasta em disco local.");return root;
 }
 public static string Destination(string root,string remote){
  if(!remote.StartsWith("scripts/")||remote.Contains("..")||remote.Contains(":"))throw new InvalidDataException("Caminho não permitido.");
  string rel=remote.StartsWith("scripts/actions/")?remote.Substring(16):remote.Substring(8);
  string full=Path.GetFullPath(Path.Combine(root,rel));
  if(!full.StartsWith(Path.GetFullPath(root).TrimEnd('\\')+"\\",StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Caminho fora da pasta de execução.");return full;
 }
 public static async Task<byte[]> Download(Catalog cat,string remote,string token){
  using(var handler=new HttpClientHandler{AllowAutoRedirect=false})using(var client=new HttpClient(handler)){
   client.Timeout=TimeSpan.FromSeconds(90);
   client.DefaultRequestHeaders.Add("User-Agent","ice-optimizer/1.0");
   string url="https://raw.githubusercontent.com/"+cat.repository+"/"+cat.@ref+"/"+string.Join("/",remote.Split('/').Select(Uri.EscapeDataString));
   using(var response=await client.GetAsync(url)){
    if(!response.IsSuccessStatusCode)throw new IOException("Download: HTTP "+(int)response.StatusCode+". Confira sua conexão e a disponibilidade do repositório.");
    byte[] bytes=await response.Content.ReadAsByteArrayAsync();Verify(bytes,cat.files[remote]);return bytes;
   }
  }
 }

 public static async Task<string> Prepare(Catalog cat,ActionItem action,string root,bool local,string token,Action<string> report){
  root=SafeRoot(root);string run=Path.Combine(root,DateTime.Now.ToString("yyyyMMdd-HHmmss")+"-"+Guid.NewGuid().ToString("N").Substring(0,8));Directory.CreateDirectory(run);
  foreach(string remote in new[]{action.file}.Concat(action.dependencies)){
   report("Preparando: "+Path.GetFileName(remote));byte[] bytes;
   if(local){string p=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,remote.Replace('/',Path.DirectorySeparatorChar));if(!File.Exists(p))throw new FileNotFoundException("Arquivo ausente no pacote offline: "+remote);bytes=File.ReadAllBytes(p);Verify(bytes,cat.files[remote]);}
   else bytes=await Download(cat,remote,token);
   string dest=Destination(run,remote);Directory.CreateDirectory(Path.GetDirectoryName(dest));File.WriteAllBytes(dest,bytes);
  }
  return Destination(run,action.file);
 }
 public static Task<int> Execute(string script,Action<string> log){return Task.Run(()=>{
  var info=new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),"cmd.exe"),"/d /s /c \"\""+script+"\"\""){
   UseShellExecute=false,CreateNoWindow=true,WorkingDirectory=Path.GetDirectoryName(script),RedirectStandardOutput=true,RedirectStandardError=true,RedirectStandardInput=true,StandardOutputEncoding=Encoding.UTF8,StandardErrorEncoding=Encoding.UTF8
  };
  // Never pass the GitHub credential to the child process.
  using(var process=new Process{StartInfo=info}){
   process.OutputDataReceived+=(s,e)=>{if(e.Data!=null)log(e.Data);};process.ErrorDataReceived+=(s,e)=>{if(e.Data!=null)log(e.Data);};
   process.Start();process.StandardInput.Close();process.BeginOutputReadLine();process.BeginErrorReadLine();process.WaitForExit();return process.ExitCode;
  }
 });}
}

public class MainWindow:Form {
 readonly Catalog catalog=Catalog.Load();
 readonly Color bg=Color.FromArgb(13,19,28),surface=Color.FromArgb(22,31,44),muted=Color.FromArgb(159,176,196),accent=Color.FromArgb(91,221,231);
 ListBox categories=new ListBox(),list=new ListBox();TextBox search=new TextBox(),root=new TextBox(),token=new TextBox();RichTextBox output=new RichTextBox();
 Label detail=new Label(),description=new Label(),status=new Label(),count=new Label();CheckBox offline=new CheckBox();Button run=new Button();bool busy;string logFile;readonly object logLock=new object();
 public MainWindow(){
  Text="ice optimizer · por Maciota";ClientSize=new Size(1120,780);MinimumSize=new Size(1000,740);StartPosition=FormStartPosition.CenterScreen;BackColor=bg;ForeColor=Color.White;Font=new Font("Segoe UI",10);AutoScaleMode=AutoScaleMode.Dpi;
  var sidebar=new Panel{Dock=DockStyle.Left,Width=218,BackColor=surface};Controls.Add(sidebar);
  Label brand=LabelAt("ice",24,26,160,55,34,accent);sidebar.Controls.Add(brand);sidebar.Controls.Add(LabelAt("optimizer",27,83,175,38,22,Color.White));sidebar.Controls.Add(LabelAt("POR MACIOTA",28,130,165,24,9,muted));
  categories.SetBounds(18,200,184,300);categories.BorderStyle=BorderStyle.None;categories.BackColor=surface;categories.ForeColor=Color.White;categories.Font=new Font("Segoe UI",12);categories.ItemHeight=40;categories.DrawMode=DrawMode.OwnerDrawFixed;categories.DrawItem+=DrawCategory;
  categories.Items.Add("Todas as ações");foreach(var c in catalog.actions.Select(a=>a.category).Distinct())categories.Items.Add(c);sidebar.Controls.Add(categories);
  var foot=LabelAt("WINDOWS 10 / 11\n\nv1.0  ·  "+catalog.actions.Length+" ações\n\nSeu PC, suas escolhas.",27,570,172,140,10,muted);sidebar.Controls.Add(foot);
  var body=new Panel{Dock=DockStyle.Fill,Padding=new Padding(28,18,28,18)};Controls.Add(body);body.BringToFront();
  var tabs=new TabControl{Dock=DockStyle.Fill};body.Controls.Add(tabs);
  var page=new TabPage("Otimizações"){BackColor=bg,ForeColor=Color.White,Padding=new Padding(18)};var access=new TabPage("Acesso e pasta"){BackColor=bg,ForeColor=Color.White,Padding=new Padding(22)};tabs.TabPages.Add(page);tabs.TabPages.Add(access);
  var grid=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=1,RowCount=8};page.Controls.Add(grid);
  foreach(int h in new[]{48,35,28,210,58,94,46})grid.RowStyles.Add(new RowStyle(SizeType.Absolute,h));grid.RowStyles.Add(new RowStyle(SizeType.Percent,100));
  grid.Controls.Add(new Label{Text="Prepare o PC do seu jeito.",Dock=DockStyle.Fill,Font=new Font("Segoe UI Semibold",23),ForeColor=Color.White},0,0);
  search.Dock=DockStyle.Fill;search.BackColor=surface;search.ForeColor=Color.White;search.BorderStyle=BorderStyle.FixedSingle;grid.Controls.Add(search,0,1);
  count.Dock=DockStyle.Fill;count.ForeColor=muted;count.Text="Buscar por nome ou categoria";grid.Controls.Add(count,0,2);
  list.Dock=DockStyle.Fill;list.BackColor=surface;list.ForeColor=Color.White;list.BorderStyle=BorderStyle.None;list.Font=new Font("Segoe UI",12);list.ItemHeight=29;list.DrawMode=DrawMode.OwnerDrawFixed;list.DrawItem+=DrawAction;grid.Controls.Add(list,0,3);
  detail.Dock=DockStyle.Fill;detail.Padding=new Padding(0,12,0,0);detail.Font=new Font("Segoe UI Semibold",15);detail.ForeColor=accent;grid.Controls.Add(detail,0,4);
  description.Dock=DockStyle.Fill;description.ForeColor=muted;grid.Controls.Add(description,0,5);
  var row=new FlowLayoutPanel{Dock=DockStyle.Fill};run.Text="Executar ação";StyleButton(run);run.Width=190;row.Controls.Add(run);status.AutoSize=true;status.Padding=new Padding(12,10,0,0);status.ForeColor=muted;status.Text="Pronto para escolher";row.Controls.Add(status);grid.Controls.Add(row,0,6);
  output.Dock=DockStyle.Fill;output.BackColor=Color.FromArgb(9,14,21);output.ForeColor=Color.FromArgb(186,210,221);output.BorderStyle=BorderStyle.None;output.ReadOnly=true;output.Font=new Font("Consolas",9);output.Text="REGISTRO DE EXECUÇÃO\nNenhuma alteração foi aplicada.\n";grid.Controls.Add(output,0,7);
  access.Controls.Add(LabelAt("Acesso aos scripts",22,24,670,50,23,Color.White));
  access.Controls.Add(LabelAt("Repositório público: "+catalog.repository,24,85,690,26,11,accent));
  access.Controls.Add(LabelAt("Download direto, sem cadastro",24,134,670,28,11,Color.White));
  token.SetBounds(24,170,670,32);token.UseSystemPasswordChar=true;token.BackColor=surface;token.ForeColor=Color.White;token.Visible=false;
  access.Controls.Add(LabelAt("Os scripts são baixados do repositório público e verificados antes de executar.\nNão é necessário informar senha, token ou criar uma conta.",24,216,685,58,10,muted));
  offline.SetBounds(24,282,690,34);offline.Text="Usar scripts locais do pacote offline (sem download)";offline.Checked=false;access.Controls.Add(offline);
  access.Controls.Add(LabelAt("Pasta de trabalho",24,338,670,30,15,Color.White));
  root.SetBounds(24,385,560,30);root.Text=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"ice optimizer","Runs");root.BackColor=surface;root.ForeColor=Color.White;access.Controls.Add(root);
  var browse=new Button{Text="Escolher..."};StyleButton(browse);browse.SetBounds(598,382,110,36);access.Controls.Add(browse);browse.Click+=(s,e)=>{using(var dialog=new FolderBrowserDialog{Description="Pasta para scripts, backups e registros"})if(dialog.ShowDialog()==DialogResult.OK)root.Text=dialog.SelectedPath;};
  access.Controls.Add(LabelAt("Esta pasta guarda os downloads, backups e registros. Os ajustes afetam o\nWindows deste computador; não otimizam uma instalação offline em outra unidade.",24,440,690,60,10,muted));
  access.Controls.Add(LabelAt("ice optimizer · por Maciota",24,554,680,34,17,accent));
  access.Controls.Add(LabelAt("Interface e integração por Maciota. Utilitários externos mantêm seus autores\ne licenças. Nenhum banco de dados ou serviço Heroku é necessário.",24,600,685,56,10,muted));
  categories.SelectedIndexChanged+=(s,e)=>Filter();search.TextChanged+=(s,e)=>Filter();list.SelectedIndexChanged+=(s,e)=>SelectAction();
  run.Click+=async(s,e)=>await RunAction();categories.SelectedIndex=0;
  FormClosing+=(s,e)=>{if(busy){e.Cancel=true;MessageBox.Show("Aguarde o término da ação antes de fechar.",Text);}else token.Clear();};
 }
 Label LabelAt(string text,int x,int y,int w,int h,int size,Color color){return new Label{Text=text,Left=x,Top=y,Width=w,Height=h,ForeColor=color,Font=new Font("Segoe UI",size)};}
 void StyleButton(Button button){button.Height=36;button.BackColor=accent;button.ForeColor=bg;button.FlatStyle=FlatStyle.Flat;button.FlatAppearance.BorderSize=0;button.Cursor=Cursors.Hand;}
 void DrawCategory(object sender,DrawItemEventArgs e){if(e.Index<0)return;bool selected=(e.State&DrawItemState.Selected)!=0;using(var b=new SolidBrush(selected?Color.FromArgb(35,64,77):surface))e.Graphics.FillRectangle(b,e.Bounds);TextRenderer.DrawText(e.Graphics,categories.Items[e.Index].ToString(),categories.Font,new Rectangle(e.Bounds.X+12,e.Bounds.Y,e.Bounds.Width-12,e.Bounds.Height),selected?accent:muted,TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis);}
 void DrawAction(object sender,DrawItemEventArgs e){if(e.Index<0)return;var a=(ActionItem)list.Items[e.Index];bool selected=(e.State&DrawItemState.Selected)!=0;using(var b=new SolidBrush(selected?Color.FromArgb(30,65,79):surface))e.Graphics.FillRectangle(b,e.Bounds);TextRenderer.DrawText(e.Graphics,a.title,list.Font,new Rectangle(e.Bounds.X+12,e.Bounds.Y,e.Bounds.Width-130,e.Bounds.Height),selected?accent:Color.White,TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis);TextRenderer.DrawText(e.Graphics,a.category,Font,new Rectangle(e.Bounds.Right-115,e.Bounds.Y,105,e.Bounds.Height),muted,TextFormatFlags.Right|TextFormatFlags.VerticalCenter);}
 void Filter(){string c=categories.SelectedItem as string;string q=search.Text.Trim();list.BeginUpdate();list.Items.Clear();foreach(var a in catalog.actions.Where(a=>(c==null||c=="Todas as ações"||a.category==c)&&(a.title+" "+a.category).IndexOf(q,StringComparison.CurrentCultureIgnoreCase)>=0))list.Items.Add(a);list.EndUpdate();count.Text=list.Items.Count+" ações · busque por nome ou categoria";if(list.Items.Count>0)list.SelectedIndex=0;else SelectAction();}
 void SelectAction(){var a=list.SelectedItem as ActionItem;detail.Text=a==null?"Nenhuma ação encontrada":a.title;description.Text=a==null?"Tente outro termo de busca.":a.warning;run.Enabled=a!=null&&!busy;}
 void Log(string line){lock(logLock){if(logFile!=null)File.AppendAllText(logFile,line+Environment.NewLine,Encoding.UTF8);}if(IsHandleCreated)BeginInvoke((Action)(()=>{output.AppendText(line+Environment.NewLine);output.SelectionStart=output.TextLength;output.ScrollToCaret();}));}
 async Task RunAction(){var a=list.SelectedItem as ActionItem;if(a==null||busy)return;
  if(!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)){MessageBox.Show("Feche e abra ice optimizer com 'Executar como administrador' para aplicar ajustes.",Text);return;}
  if(MessageBox.Show(a.title+"\n\n"+a.warning+"\n\nExecutar esta ação?",Text,MessageBoxButtons.YesNo,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2)!=DialogResult.Yes)return;
  busy=true;run.Enabled=false;status.Text="Preparando arquivos...";string accessToken=token.Text.Trim();bool local=offline.Checked;string selectedRoot=root.Text;
  try{
   Payload.SafeRoot(selectedRoot);
   string script=await Payload.Prepare(catalog,a,selectedRoot,local,accessToken,Log);accessToken=null;
   logFile=Path.Combine(Path.GetDirectoryName(script),"execution.log");Log("ice optimizer · por Maciota | "+a.title+" | "+DateTime.Now.ToString("s"));status.Text="Executando...";
   // Recheck all prepared assets just before launching.
   foreach(string file in new[]{a.file}.Concat(a.dependencies))Payload.Verify(File.ReadAllBytes(Payload.Destination(Path.GetDirectoryName(script),file)),catalog.files[file]);
   int code=await Payload.Execute(script,Log);Log("Processo encerrado. Código: "+code+". Scripts legados podem conter falhas parciais: revise este registro.");Log("Registro: "+logFile);status.Text=code==0?"Encerrado · revise o registro":"Encerrado com erro · "+code;
  }catch(Exception ex){status.Text="Ação interrompida";Log("ERRO: "+ex.Message);MessageBox.Show(ex.Message,Text,MessageBoxButtons.OK,MessageBoxIcon.Error);}
  finally{accessToken=null;logFile=null;busy=false;SelectAction();}
 }
 public void Render(string file){CreateControl();ShowInTaskbar=false;Opacity=0;Show();Application.DoEvents();using(var bitmap=new Bitmap(Width,Height)){DrawToBitmap(bitmap,new Rectangle(0,0,Width,Height));bitmap.Save(file);}Hide();}
 public void UiTest(){categories.SelectedItem="Jogos";search.Text="rust";if(list.Items.Count!=1||((ActionItem)list.Items[0]).id!="priorizar_rust")throw new Exception("Search/category filtering failed");search.Text="zzzzzz";if(run.Enabled)throw new Exception("Empty selection enabled execution");}
}
public static class Program {
 [STAThread] public static int Main(string[] args){
  ServicePointManager.SecurityProtocol=SecurityProtocolType.Tls12;Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
  try{
   if(args.Length==2&&args[0]=="--download-test"){
    var cat=Catalog.Load();foreach(string id in new[]{"priorizar_rust","opcao22"}){var a=cat.actions.Single(x=>x.id==id);Payload.Prepare(cat,a,Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[1])),"download-test"),false,null,x=>{}).GetAwaiter().GetResult();}
    File.WriteAllText(args[1],"PASS: public HTTPS downloads and SHA-256 verification, Rust script and complete ISLC dependency tree; no script executed.");return 0;
   }
   if(args.Length>0&&args[0]=="--self-test"){
    var c=Catalog.Load();if(c.actions.Length<100||c.actions.Select(a=>a.id).Distinct().Count()!=c.actions.Length)throw new Exception("Invalid catalog");
    byte[] b=Encoding.UTF8.GetBytes("verified");var f=new FileInfoEntry{sha256=Payload.Hash(b),size=b.Length};Payload.Verify(b,f);bool rejected=false;try{Payload.Verify(new byte[]{1},f);}catch(InvalidDataException){rejected=true;}if(!rejected)throw new Exception("Corruption accepted");
    foreach(string p in new[]{"C:\\bad%PATH%","C:\\bad!name","\\\\server\\share"}){rejected=false;try{Payload.SafeRoot(p);}catch(ArgumentException){rejected=true;}if(!rejected)throw new Exception("Unsafe path accepted");}
    using(var window=new MainWindow())window.UiTest();
    string testRoot=Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[1])),"test runs "+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(testRoot);
    string script=Path.Combine(testRoot,"harmless.bat");File.WriteAllText(script,"@echo off\r\necho ICE_TEST_OK\r\nexit /b 7\r\n");var lines=new List<string>();int code=Payload.Execute(script,l=>{lock(lines)lines.Add(l);}).GetAwaiter().GetResult();if(code!=7||!lines.Contains("ICE_TEST_OK"))throw new Exception("Process output/exit test failed");
    var act=c.actions.Single(a=>a.id=="ping");string prepared=Payload.Prepare(c,act,testRoot,true,null,l=>{}).GetAwaiter().GetResult();if(!File.Exists(Path.Combine(Path.GetDirectoryName(prepared),"DnsJumper.exe")))throw new Exception("Dependency layout invalid");
    if(args.Length>1)File.WriteAllText(args[1],"PASS: embedded catalog, SHA-256 tampering rejection, unsafe paths, GUI search/category/empty selection. No optimization executed.");return 0;
   }
   using(var window=new MainWindow()){if(args.Length==2&&args[0]=="--render"){window.Render(args[1]);return 0;}Application.Run(window);}return 0;
  }catch(Exception e){if(args.Length>1)File.WriteAllText(args[args.Length-1],e.ToString());else MessageBox.Show(e.Message,"ice optimizer");return 1;}
 }
}
