# what is async and await

practically, it is a mechanism that allows you to hang your code up, waiting until the awatable method to be done before proceed into next part of your method.

for example:

    async method A
        do sth
        await method B
        do sth
        var a = methodAlpha
        var b = methodBeta
        await Task.WhenAll(a, b)
        done

    async method B
        await do request, takes looooooooooooong time
        done

    async methodAlpha
        something serious
        blahblahblah

    async methodBeta
        something dangerous
        blahblahblah

its wont jam your thread, the thread might proceed other stuffs all together, it just jamming the method that awaiting for sub actions to be done. The awaited I/O executes on the other machine; while waiting, zero threads are occupied

on ASP.NET Core, it is different from what async await in unity, it is not on one single thread, it will hanging up and might picked up by one of the thread inside threadding pool.

    ASP.NET Core, it don't have synchronization context, the continuation is picking up by any thread.
    Unity, it has synchronization context, any awaitable will loop back to main thread, causing it concurrnet undernease one single thread.

Method itself that awaiting is a state machine, like someone ate a bite of pie, founds it is not heated up, so putting it into microwave oven, 
when done, next random guy passing by, pick it up and make a bite again