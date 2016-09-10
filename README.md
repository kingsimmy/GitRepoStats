# GitRepoStats
Count the number of lines added and deleted per author on a Git repository.

This is a commandline application which can be used to gather statistics about a checked out Git repository. 
Multiple repositories can be analysed in one run by passing the repository paths separated by spaces.

The default output mode is text.
To have the output as an html page pass option -html

By default output appears on std out.
To have output written to a file pass option -outFile <filename>

Example command line for a single repository generating text output:
```
>GitRepoStats.CommandLine.exe C:\src\html-generator
```
```
C:\src\html-generator\.git\
Hugh Bellamy <hughbellars@gmail.com> 3 commits. 8350 added. 25 deleted.
Peter Jas <necmon@yahoo.com> 1 commits. 1226 added. 1368 deleted.
kingsimmy <kingsimmy@users.noreply.github.com> 1 commits. 43 added. 1 deleted.
.gitignore 1 files totalling 252 lines.
.yml 2 files totalling 147 lines.
.md 2 files totalling 53 lines.
 1 files totalling 22 lines.
.png 1 files totalling 0 lines.
.json 3 files totalling 67 lines.
.sln 1 files totalling 33 lines.
.cs 245 files totalling 7831 lines.
.xproj 2 files totalling 40 lines.
```
Example command line for multiple repositores generating html output to a file:
```
>GitRepoStats.CommandLine.exe C:\src\ChakraCore C:\src\html-generator C:\src\monaco-editor -outFile "C:\src\report.html" -html
```
![alt tag](https://i.imgur.com/gXMRabQ.png)
