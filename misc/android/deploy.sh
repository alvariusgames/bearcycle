BASEDIR=$(dirname "$0")
adb install -r $BASEDIR/../../dist/android/BearsOnATVs.apk && adb shell monkey -p com.ludditegames.bearsonatvs -c android.intent.category.LAUNCHER 1 && adb logcat 
