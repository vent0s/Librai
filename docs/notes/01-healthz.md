# 01 GET /healthz

## 1. what does it solve?
    it tells whether the server, or the entire service is alive or not, 
    /healthz itself is an industrial standard for k8s/docker health probe, 
    the -z is the traditional suffix

## 2. api design strategy and industrial standard implementation
    why GET: 
        its an idempotent action, do it once or multiple times comes the same result, safe and having no significant issue.
        network is unreliable, if client having latency, it might fire request multiple times, if we are using non-idempotent method,
        it result in multiple request, causes issue.
    why json:
        its a machine readable data structure, you can reconstruct and translate into meaningful object or structure that machine could easily handle,
    
    why record:
        one line, immutable data passing structure is DTO friendly.
    why make it a NetService:
        everything has to be a NetService, this is a standardized engineering design that in future,
        we want make features that serves a similar purpose to be decoupling and cohesion, so making any changes won't affect the grand blueprint of the codebase

## 3. validation
    curl http://localhost:5074/healthz
    return:
    200
    OK
    {"status":"ok","checkedAt":"20xx-xx-xxTxx:xx:xx.xxxx..."}


## 4. relevant code
NetServices/Healthz.cs

namespace LibrAI.Api.NetServices
{

    public static class HealthzService
    {
        public static HealthzResponse GetHealthz()
        {
            return new HealthzResponse("ok", DateTime.UtcNow);
        }
    }

    public record HealthzResponse(string Status, DateTime CheckedAt);
}

LibrAI.Api/Program.cs
app.MapGet("/healthz", () => HealthzService.GetHealthz());
    