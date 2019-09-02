# Splitio flakey .net client creation demo

This is a tiny project to play around with the split.io
.net library, which sometimes falls over when creating a
client.

To run:

    dotnet run <your api key> <split name> <split key>

If it works, you should see your split 'treatment' printed
to console. Sometimes, creating a client fails for some reason,
and you'll see "SDK was not ready in 5000 milliseconds'. After
this occurs, it seems impossible to create a working client
until the entire process is restarted.
