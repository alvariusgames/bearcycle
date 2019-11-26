#!/usr/bin/python3

from pathlib import Path
import traceback
import pexpect
import sys

def execute(cmd, initial_std_line=None):
    analyzer = pexpect.spawn(cmd)
    analyzer.expect("\n>")
    print(analyzer.before.decode("utf-8"), end="")
    print(analyzer.after.decode("utf-8"), end="")
    if initial_std_line:
        analyzer.sendline("open " + initial_std_line)
        analyzer.expect("\n>")
        print(analyzer.before.decode("utf-8"), end="")
        print(analyzer.after.decode("utf-8"), end="")
 
    analyzer.interact()

path = Path('/home/david/.wine/drive_c/Program\ Files\ \(x86\)/LiteDbShell414/LiteDB.Shell.exe')

try:
    print("Press Ctrl+C (KeyboardInterrupt) TWICE to quit at any time.\n-----")
    while True:
        try:
            if len(sys.argv)>1:
                initial_std_line = sys.argv[1]
            else:
                initial_std_line = None
            execute("wine64 {}".format(str(path)), initial_std_line)
        except Exception as e:
            print(traceback.format_exc())
            print("[Python] Command failed, restarting DBLite instance...\n-----\n")
            
            pass
        """
        #, shell=True, stdout=PIPE, stdin=PIPE, stderr=PIPE)

        for stdout_line in iter(process.stdout.readline, ""):
            print(stdout_line, end="")
        process.stdout.close()
        return_code = popen.wait()
        #print(process.communicate(input=b"hello")[0])
        """
except KeyboardInterrupt as k:
    pass

print("Python process exiting...")
