using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class BatchResult {public string id,title,status,folder;public int code;}
public static class BatchPlan {
 public static List<string> Problems(IEnumerable<ActionItem> source){var a=source.ToArray();var ids=new HashSet<string>(a.Select(x=>x.id));var problems=new List<string>();
  if(a.Length==0)problems.Add("Marque pelo menos uma ação.");
  foreach(var item in a){if(a.Length>1&&item.standalone)problems.Add(item.title+": execute separadamente.");foreach(var id in item.conflicts??new string[0])if(ids.Contains(id)&&string.CompareOrdinal(item.id,id)<0)problems.Add(item.title+" + "+a.Single(x=>x.id==id).title+": escolha apenas uma.");}
  return problems;
 }
 public static ActionItem[] Order(IEnumerable<ActionItem> source){return source.OrderBy(a=>a.id=="restore"?0:1).ToArray();}
 public static async Task<List<BatchResult>> Run(ActionItem[] items,Func<ActionItem,Task<string>> prepare,Func<ActionItem,string,Task<int>> execute,Func<bool> cancel,Action<string,int,int,ActionItem> progress,bool stopOnError){
  var issues=Problems(items);if(issues.Count>0)throw new InvalidOperationException(string.Join("\n",issues));items=Order(items);var paths=new Dictionary<string,string>();var results=new List<BatchResult>();
  // Every payload is ready before the first system change (network resets can disconnect the PC).
  for(int i=0;i<items.Length;i++){if(cancel())return results;progress("download",i,items.Length,items[i]);paths[items[i].id]=await prepare(items[i]);}
  for(int i=0;i<items.Length;i++){
   if(cancel())break;var a=items[i];progress("execute",i,items.Length,a);int code;
   try{code=await execute(a,paths[a.id]);}catch(Exception e){results.Add(new BatchResult{id=a.id,title=a.title,status="Não foi possível executar: "+e.Message,code=-1,folder=Path.GetDirectoryName(paths[a.id])});if(stopOnError)break;else continue;}
   results.Add(new BatchResult{id=a.id,title=a.title,status=code==0?"Concluída com sucesso":"A ação terminou com falha. Código técnico: "+code,code=code,folder=Path.GetDirectoryName(paths[a.id])});progress("complete",i+1,items.Length,a);
   if(code!=0&&stopOnError)break;
  }return results;
 }
}
