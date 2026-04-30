dotnet test \
  --logger "console;verbosity=normal" \
  --filter "Layer=API" \
  --logger "trx"