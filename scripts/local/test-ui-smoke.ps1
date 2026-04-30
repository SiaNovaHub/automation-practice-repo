dotnet test `
  --logger "console;verbosity=normal" `
  --filter "Layer=UI&Type=Smoke" `
  --logger "trx"