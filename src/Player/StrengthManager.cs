using Godot;
using System;

public enum StrengthManagerState { WEAK, // 0%-25%
                                   WEARY, // 25%-50%
                                   GOOD, // 50%-75%
                                   GREAT // 75%-100%
}

public class StrengthManager : FSMNode2D<StrengthManagerState>{
    public override StrengthManagerState InitialState { get { return StrengthManagerState.GOOD;} set {}}
    private float strength = 2f / 3f;
    public float Strength { 
        get {
            return this.strength;}
        set{
            this.strength = value;
            if(this.strength > 1f){
                this.strength = 1f;}
            if(this.strength < 0f){
                this.strength = 0f;}}}

    public override void UpdateState(float delta){

    }

    public override void ReactToState(float delta){

    }

    public override void ReactStateless(float delta){

    }

}
