# Resourcerer template

Solution template based on Resourcerer project. It's generating boilerplate solution structure including [ReinforcedTypings](https://github.com/reinforced/Reinforced.Typings) exports for dtos. View project and database setup details in the `master` branch readme file.

## Template usage

Check if template is installed: `dotnet new list`
Run `dotnet new rsrc -o "MyPrefix"` to scaffold new solution.

## Template updates

After template is updated, from root run:
```
cd ./.template.config
dotnet new install .
dotnet new install . --force (if installed already)
```