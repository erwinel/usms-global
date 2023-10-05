# usms-global

Global scripts for USMS ServiceNow instances

## Dev Setup

This is intended to be developed using VS Code. See [/.vscode/extensions.json](./.vscode/extensions.json) for a list of recommended extensions. Dependencies are mananged using [npm](https://www.npmjs.com/).

Run `npm install` from the root folder of this repository to install dependencies.

### Github Codespaces Initialization Issue

If the language service fails to recognize namespaces from added packages such as `Microsoft.EntityFrameworkCore`, you may need to do the following:

1. Remove subdirectories `util/src/SnTsTypeGenerator/bin` and `util/src/SnTsTypeGenerator/obj`
2. Execute the command `dotnet restore util/src`
3. From the command palette, select `.NET: Restart Language Server`.

### SNC Typings Submodule Maintenance

#### Initialize Submodule after Repository Clone

This is for pulling down the repository contents from the remote.

These commands assume you are starting out in the root of the respository.

If the submodules were already included when the repository was cloned, you only need to do the third command if the submodule is not on any branch.

```sh
git submodule update --init --recursive
cd types/snc
git checkout master
```

### Update Submodule from Origin

This is for pulling down the latest changes from the remote repository.

These commands assume you are starting out in the root of the respository.

```sh
cd types/snc
git pull --rebase
cd ../..
git commit -am "Updated types/snc submodule to latest revision"
```

### Push Submodule Updates to Origin

If you make changes to the contents of the `types/snc` submodule, you can use these commands to commit the changes and push them to the remote repository.

```sh
cd types/snc
git add -A
git commit -am "[Your message here]"
git push
cd ../..
git commit -am "[Your message here]"
```
