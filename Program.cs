using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(x =>
{
    x.AddPolicy("AllowAll", y =>
    {
        y.AllowAnyOrigin();
        y.AllowAnyHeader();
        y.AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

// vars
var workflowDefs = new Dictionary<string, WorkflowDefinition>();
var workflowInstances = new Dictionary<string, WorkflowInstance>();

// endpoint for defs
app.MapPost("/defs", (WorkflowDefinition def) =>
{
    if (workflowDefs.ContainsKey(def.Id))
    {
        return Results.BadRequest(new { error = "Definition already exists" });
    }
    else
    {
        workflowDefs[def.Id] = def;
        return Results.Ok(new { status = "added", id = def.Id });
    }
});

// make new instance
app.MapPost("/instances", (string defId) =>
{
    if (!workflowDefs.ContainsKey(defId))
    {
        return Results.BadRequest(new { error = "Definition not found" });
    }

    var def = workflowDefs[defId];
    State init = null;
    foreach (var s in def.States)
    {
        if (s.IsInitial)
        {
            init = s;
            break;
        }
    }

    if (init == null)
    {
        return Results.BadRequest(new { error = "Initial state missing" });
    }

    var id = Guid.NewGuid().ToString();
    var inst = new WorkflowInstance();
    inst.Id = id;
    inst.DefId = defId;
    inst.CurrentState = init.Id;
    inst.History = new List<HistoryItem>();

    workflowInstances[id] = inst;

    return Results.Ok(inst);
});

// do action
app.MapPost("/instances/{id}/actions/{actionId}", (string id, string actionId) =>
{
    if (!workflowInstances.ContainsKey(id))
    {
        return Results.NotFound(new { error = "Instance not found" });
    }

    var inst = workflowInstances[id];
    var def = workflowDefs[inst.DefId];

    ActionDef act = null;
    foreach (var a in def.Actions)
    {
        if (a.Id == actionId)
        {
            act = a;
            break;
        }
    }

    if (act == null)
    {
        return Results.BadRequest(new { error = "Invalid action" });
    }

    if (!act.FromStates.Contains(inst.CurrentState))
    {
        return Results.BadRequest(new { error = "Action not allowed from current state" });
    }

    var hist = new HistoryItem();
    hist.ActionId = actionId;
    hist.Timestamp = DateTime.UtcNow;
    inst.History.Add(hist);

    inst.CurrentState = act.ToState;

    return Results.Ok(inst);
});

app.Run();

// classes
public record WorkflowDefinition(string Id, List<State> States, List<ActionDef> Actions);
public record State(string Id, bool IsInitial, bool IsFinal);
public record ActionDef(string Id, List<string> FromStates, string ToState);
public class WorkflowInstance
{
    public string Id { get; set; }
    public string DefId { get; set; }
    public string CurrentState { get; set; }
    public List<HistoryItem> History { get; set; }
}
public class HistoryItem
{
    public string ActionId { get; set; }
    public DateTime Timestamp { get; set; }
}
