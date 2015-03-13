#This is documentation for the **http-command** project.

# Introduction #

HttpCommand.exe is a simple command-line utility for MS Windows.

# Details #

  * HttpCommand - Copyright (c) 2011 Paul Isaac
  * Utility to download files from a web server.

  * Useage: HttpCommand {flags} [http:\\uri] {local-target}
  * Flags: -S = recurse subdirectories

  * For HTTP resources the GET method is used.
  * For FTP resources the RETR command is used.

# Example #

HttpCommand -S http:\\http-command.googlecode.com/svn/trunk .