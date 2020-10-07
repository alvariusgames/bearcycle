using Godot;
using System;

public class TransitionEffectRunner : FSMNode2D<SceneTransitionEffect>{
    //This is needed since SceneTransitioner updates state before `_Process()`
    private SceneTransitionEffect initialState = SceneTransitionEffect.DIRECT_CUT;
    public override SceneTransitionEffect InitialState { get { return this.initialState;} set{this.initialState = value;}}
    public Viewport ToViewport;
    public Viewport FromViewport;
    public ViewportContainer FromViewportContainer;
    public ViewportContainer ToViewportContainer;
    public Node ToScene;
    public Node FromScene;
    public Node TransitionerScene;
    public Boolean FadeOutAudio;
    public float NumSeconds;
    public float elapsedSeconds = 0f;
    public override void UpdateState(float delta){}

    public override void ReactStateless(float delta){
        this.elapsedSeconds += delta;
    }

    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case SceneTransitionEffect.DIRECT_CUT:
                var tree = this.GetTree();
                var RootNode = tree.GetRoot();
                ToViewport.RemoveChild(ToScene);
                FromViewport.RemoveChild(FromScene);
                RootNode.RemoveChild(TransitionerScene);
                TransitionerScene.CallDeferred("free");
                FromScene.CallDeferred("free");
                RootNode.AddChild(ToScene);

                TransitionerScene.PauseMode = Node.PauseModeEnum.Inherit;
                ToScene.PauseMode = Node.PauseModeEnum.Inherit;
                FromScene.PauseMode = Node.PauseModeEnum.Inherit;

                if(FadeOutAudio){
                    SoundHandler.EndTempSetAllStreamAndSample();}

                tree.Paused = false;
                break;
            case SceneTransitionEffect.FADE_INTO:
                var effectiveTransparency = FromViewportContainer.GetModulate()[3];
                effectiveTransparency -= (delta / NumSeconds);
                this.FromViewportContainer.SetModulate(new Color(1,1,1, effectiveTransparency));
                if(FadeOutAudio){
                    SoundHandler.TempSetAllStreamAndSample(1 - (this.elapsedSeconds / NumSeconds));}
                break;
            case SceneTransitionEffect.FADE_BLACK:
                var halfTime = NumSeconds / 2f;
                var step = delta / halfTime;
                var fromEffectiveColor = FromViewportContainer.GetModulate();
                var toEffectiveColor = ToViewportContainer.GetModulate();
                if(fromEffectiveColor == new Color(1,1,1,1)){
                    FromViewportContainer.SetModulate(new Color(0.9999f, 0.9999f, 0.9999f, 0.9999f));
                }
                if (toEffectiveColor == new Color(1,1,1,1)){
                    ToViewportContainer.SetModulate(new Color(0,0,0,0.9999f));
                }
                if(this.elapsedSeconds < halfTime){
                    this.FromViewportContainer.SetModulate(new Color(fromEffectiveColor.r - step,
                                                                     fromEffectiveColor.g - step,
                                                                     fromEffectiveColor.b - step,
                                                                     fromEffectiveColor.a - step));
                } else {
                    this.ToViewportContainer.SetModulate(new Color(toEffectiveColor.r + step,
                                                                    toEffectiveColor.g + step,
                                                                    toEffectiveColor.b + step,
                                                                    toEffectiveColor.a));}
                if(FadeOutAudio){
                    SoundHandler.TempSetAllStreamAndSample(1 - (this.elapsedSeconds / NumSeconds));}
                return;
            default:
                return;
        

        //Finally switch to the toScene

    }
  }
//  {
//      
//  }
}
