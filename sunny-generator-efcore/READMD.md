```shell
rm -rf Generator && dotnet ef dbcontext scaffold "Server=" \
MySql.EntityFrameworkCore \
--context-dir Generator/Repository \
--output-dir Generator/Model \
--context AppDbContext \
--force \
-d \
--namespace WishServer
```