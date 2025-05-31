# FolderSynchronization

This is a C# console application for periodically synchronizing the contents of 2 folders and logging related events to the console and provided log file.

## Usage
1. Either build from source code or [download one of the releases](https://github.com/Meatnyan/FolderSynchronization/releases).
2. Run FolderSync.exe using the Windows Command Prompt (cmd). Provide 4 arguments (enclosed in quotes and separated by spaces):
   - Source folder path;
   - Replica folder path;
   - Synchronization interval (in ms);
   - Log file path.

Enclosing the synchronization interval or paths that don't contain whitespace is optional.
Example of correct syntax:

*FolderSync.exe "C:\Source Folder" "C:\Replica Folder" 5000 "C:\Log File Folder\Log File.log"*

3. The program will continuously synchronize all file operations (adding, deleting, modifying, renaming) between the two folders and log each event to the console and log file. The "Source" folder will never be modified by the program - only the "Replica" folder will.
4. To stop synchronizing, simply terminate the process (e.g. close the running cmd window).

![image](https://github.com/user-attachments/assets/70ff2d6f-3970-4e30-a945-82f643a417c4)
