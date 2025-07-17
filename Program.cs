using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ✅ Enable CORS globally
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Apply CORS policy
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Workflow Definitions
Dictionary<string, WorkflowDefinition> workflowDefs = new();
Dictionary<string, WorkflowInstance> workflowInstances = new();

// POST /defs: Add a new workflow definition
app.MapPost("/defs", (WorkflowDefinition def) =>
{
    if (workflowDefs.ContainsKey(def.Id)) return Results.BadRequest(new { error = "Definition already exists" });
    workflowDefs[def.Id] = def;
    return Results.Ok(new { status = "added", id = def.Id });
});

// POST /instances?defId=pizza_order: Create a new instance
app.MapPost("/instances", (string defId) =>
{
    if (!workflowDefs.ContainsKey(defId)) return Results.BadRequest(new { error = "Definition not found" });
    var def = workflowDefs[defId];
    var initialState = def.States.FirstOrDefault(s => s.IsInitial);
    if (initialState == null) return Results.BadRequest(new { error = "Initial state missing" });
    var instance = new WorkflowInstance
    {
        Id = Guid.NewGuid().ToString(),
        DefId = defId,
        CurrentState = initialState.Id,
        History = new List<HistoryItem>()
    };
    // workflowInstances.Clear(); // ❌ remove all old orders
    workflowInstances[instance.Id] = instance;
    return Results.Ok(instance);
});

// POST /instances/{id}/actions/{actionId}
app.MapPost("/instances/{id}/actions/{actionId}", (string id, string actionId) =>
{
    if (!workflowInstances.TryGetValue(id, out var instance)) return Results.NotFound(new { error = "Instance not found" });
    var def = workflowDefs[instance.DefId];
    var action = def.Actions.FirstOrDefault(a => a.Id == actionId);
    if (action == null) return Results.BadRequest(new { error = "Invalid action" });
    if (!action.FromStates.Contains(instance.CurrentState)) return Results.BadRequest(new { error = "Action not allowed from current state" });

    instance.History.Add(new HistoryItem { ActionId = actionId, Timestamp = DateTime.UtcNow });
    instance.CurrentState = action.ToState;
    return Results.Ok(instance);
});

app.Run();

// Models
record WorkflowDefinition(string Id, List<State> States, List<ActionDef> Actions);
record State(string Id, bool IsInitial, bool IsFinal);
record ActionDef(string Id, List<string> FromStates, string ToState);
record WorkflowInstance
{
    public string Id { get; set; }
    public string DefId { get; set; }
    public string CurrentState { get; set; }
    public List<HistoryItem> History { get; set; }
}
record HistoryItem
{
    public string ActionId { get; set; }
    public DateTime Timestamp { get; set; }
}
