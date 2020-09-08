# RediSearch sample project using ASP.NET Core

In this project you may see simple mechanism of autocomplete using RediSearch. In common you fullfil some products catalog and you need to have possibility to rapidly look for products by term that user typed on client-side. So here it is =)

General purpose is to get familiar with RediSearch, but in future it may grow into something interesting


## How to start

#### CLI
Clone repo, start Redis with `ft` module  and run:

```
cd ./src
dotnet run
```


## How to use
In sorces folder you may find [app.http](https://github.com/flour/redisearch_sample/blob/master/src/Autocomplete.Api/app.http) file that can be use with `REST client` extension for VS Code. Or you can run Postman and run same requests

