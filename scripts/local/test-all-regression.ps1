dotnet test `
  --logger "console;verbosity=normal" `
  --filter "Type=Regression" `
  --logger "trx"