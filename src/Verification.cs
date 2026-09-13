using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public static class Verification {
 static void Check(bool value,string message){if(!value)throw new Exception(message);}
 public static async Task BatchTests(){
  var cat=Catalog.Load();Check(cat.actions.All(a=>!string.IsNullOrWhiteSpace(a.description)),"Missing descriptions");
  var games=cat.actions.Where(a=>a.category=="Jogos").Take(3).ToArray();var trace=new List<string>();bool stop=false;
  Func<ActionItem,Task<string>> prep=a=>{trace.Add("prepare:"+a.id);return Task.FromResult("C:\\test\\"+a.id+".bat");};
  var result=await BatchPlan.Run(games,prep,(a,p)=>{trace.Add("execute:"+a.id);return Task.FromResult(0);},()=>stop,(a,b,c,d)=>{},true);
  Check(result.Count==3,"Batch did not run all actions");Check(trace.Take(3).All(x=>x.StartsWith("prepare")),"Execution before all downloads");Check(trace.Skip(3).SequenceEqual(games.Select(a=>"execute:"+a.id)),"Not sequential");
  trace.Clear();result=await BatchPlan.Run(games,prep,(a,p)=>{stop=true;return Task.FromResult(0);},()=>stop,(a,b,c,d)=>{},true);Check(result.Count==1,"Cancel did not stop following actions");
  stop=false;result=await BatchPlan.Run(games,prep,(a,p)=>Task.FromResult(1),()=>stop,(a,b,c,d)=>{},true);Check(result.Count==1,"Failure did not stop queue");
  result=await BatchPlan.Run(games,prep,(a,p)=>Task.FromResult(1),()=>stop,(a,b,c,d)=>{},false);Check(result.Count==3,"Continue on error failed");
  bool executed=false;try{await BatchPlan.Run(games,a=>{throw new Exception("download failure");},(a,p)=>{executed=true;return Task.FromResult(0);},()=>false,(a,b,c,d)=>{},true);}catch(Exception){}Check(!executed,"Download failure applied changes");
  var incompatible=cat.actions.Where(a=>a.id=="ativarmemoria"||a.id=="desativarmemoria").ToArray();Check(BatchPlan.Problems(incompatible).Count>0,"Opposing actions allowed");
  Check(BatchPlan.Problems(new[]{cat.actions.Single(a=>a.id=="opcao34"),games[0]}).Count>0,"Reboot included in batch");
  Check(BatchPlan.Order(new[]{games[0],cat.actions.Single(a=>a.id=="restore")})[0].id=="restore","Restore not first");
 }
}
