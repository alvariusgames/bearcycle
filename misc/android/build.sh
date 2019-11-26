BASEDIR=$(dirname "$0")
rm $BASEDIR/../../dist/android/BearsOnATVs.apk
godot-beta --export Android $BASEDIR/../../dist/android/BearsOnATVs.apk 
