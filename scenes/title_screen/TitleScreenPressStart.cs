using Godot;
using System;

public enum TitleScreenState { AWAITING_PRESS_START, TRANSITIONING_TO_MAIN_MENU, 
                               IN_MAIN_MENU, TRANSITIONING_TO_SETTINGS, IN_SETTINGS,
                               IN_CHANGE_LANGUAGE, IN_MANAGE_GAME_DATA}
public class TitleScreenPressStart : FSMNode2D<TitleScreenState>
{

    public override TitleScreenState InitialState { get { return TitleScreenState.AWAITING_PRESS_START; }}

    private const int BUTTON_GRID_WIDTH = 1;
    private const int BUTTON_GRID_HEIGHT = 3;
    TouchScreenButton[,] ButtonGrid = new TouchScreenButton[BUTTON_GRID_WIDTH, BUTTON_GRID_HEIGHT];
    Tuple<int, int> currentGridIndex = Tuple.Create(-1,-1);

    private Sprite Background1;
    private Sprite Background2;
    private Sprite Logo;

    private VBoxContainer MainMenuVBoxContainer;
    private HoverableTouchScreenButton PlayButton;
    private HoverableTouchScreenButton ExitButton;
    private HoverableTouchScreenButton SettingsButton;

    private VBoxContainer SettingsVboxContainer;
    private HoverableTouchScreenButton ManageGameDataButton;
    private HoverableTouchScreenButton ChangeLanguageButton;
    private HoverableTouchScreenButton BackToMainMenuButton1;

    private VBoxContainer ManageGameDataVboxContainer;
    private HoverableTouchScreenButton ClearGameDataButton;
    private HoverableTouchScreenButton ChangeGameSlotButton;
    private Label ActiveGameSlot;
    private HoverableTouchScreenButton BackToSettings2;

    private VBoxContainer ChangeLanguageContainer;
    private OptionButton SupportedLocalesOptionButton;
    private HoverableTouchScreenButton BackToSettings1;


    private Label PressStartLabel;
    private Label VersionLabel;

    private float transitionCounter = 0f;
    private float transitionDurationS = 0.75f;

    private const float logoRightSideOfScreenPercentagePos = 0.3f;
    private float numPixelsToMoveRight = (OS.GetWindowSize().x * (0.5f - logoRightSideOfScreenPercentagePos));


    public override void _Ready(){
       SoundHandler.PlayStream<MyAudioStreamPlayer>(this,
            new string[] {"res://media/music/bears_on_atvs_theme_1.ogg"},
            Loop: true,
            SkipIfAlreadyPlaying: true);
        foreach(Node child in this.GetChildren()){
            if(child is Sprite){
                if(child.Name.ToLower().Contains("one")){
                    this.Background1 = (Sprite)child;}
                else if(child.Name.ToLower().Contains("two")){
                    this.Background2 = (Sprite)child;}
                else if(child.Name.ToLower().Contains("logo")){
                    this.Logo = (Sprite)child;}}
            if(child is Label){
                if(child.Name.ToLower().Contains("start")){
                    this.PressStartLabel = (Label)child;
                    this.PressStartLabel.Text = (String)Strings.UI["PressStart"];}
                if(child.Name.ToLower().Contains("version")){
                    this.VersionLabel = (Label)child;
                    this.VersionLabel.Text = main.VERSION;}}
            if(child is VBoxContainer && child.Name.ToLower().Contains("mainmenu")){
                this.MainMenuVBoxContainer = (VBoxContainer)child;
                foreach(Node child2 in child.GetChildren()){
                    if(child2.Name.ToLower().Contains("play")){
                        var playLabel = (Label)child2;
                        playLabel.Text = (String)Strings.UI["Play"];
                        this.PlayButton = (HoverableTouchScreenButton)playLabel.GetChild(0);}
                    if(child2.Name.ToLower().Contains("exit")){
                        var exitLabel = (Label)child2;
                        exitLabel.Text = (String)Strings.UI["Exit"];
                        this.ExitButton = (HoverableTouchScreenButton)exitLabel.GetChild(0);}
                    if(child2.Name.ToLower().Contains("settings")){
                        var settingsLabel = (Label)child2;
                        settingsLabel.Text = (String)Strings.UI["Settings"];
                        this.SettingsButton = (HoverableTouchScreenButton)settingsLabel.GetChild(0);}
                    }}
            if(child is VBoxContainer && child.Name.ToLower().Contains("settings")){
                this.SettingsVboxContainer = (VBoxContainer)child;
                foreach(Node child2 in child.GetChildren()){
                    if(child2.Name.ToLower().Contains("manage")){
                        var manageLabel = (Label)child2;
                        manageLabel.Text = (String)Strings.UI["ManageGameData"];
                        this.ManageGameDataButton = (HoverableTouchScreenButton)child2.GetChild(0);}
                    if(child2.Name.ToLower().Contains("mainmenu")){
                        this.BackToMainMenuButton1 = (HoverableTouchScreenButton)child2.GetChild(0);}
                    if(child2.Name.ToLower().Contains("language")){
                        var langLabel = (Label)child2;
                        langLabel.Text = (String)Strings.UI["ChangeLanguage"];
                        this.ChangeLanguageButton = (HoverableTouchScreenButton)child2.GetChild(0);
                    }}}
            if(child is VBoxContainer && child.Name.ToLower().Contains("language")){
                this.ChangeLanguageContainer = (VBoxContainer)child;
                foreach(Node child2 in child.GetChildren()){
                    if(child2.Name.ToLower().Contains("mainmenu")){
                        this.BackToSettings1 = (HoverableTouchScreenButton)child2.GetChildren()[0];
                    }
                    if(child2.Name.ToLower().Contains("language")){
                        this.SupportedLocalesOptionButton = (OptionButton)child2;
                        this.SupportedLocalesOptionButton.Theme = new Theme();
                        this.SupportedLocalesOptionButton.Theme.DefaultFont = new DynamicFont();
                        ((DynamicFont)this.SupportedLocalesOptionButton.Theme.DefaultFont).FontData = (DynamicFontData)GD.Load("res://media/fonts/en_US.ttf");
                        ((DynamicFont)this.SupportedLocalesOptionButton.Theme.DefaultFont).Size = 32;
                        this.SupportedLocalesOptionButton.Clear();
                        int i = 0;
                        int currentLocaleId = 0;
                        foreach(var locale in Strings.SupportedLocales()){
                            this.SupportedLocalesOptionButton.AddItem(locale, i);
                            if(locale == DbHandler.Globals.Locale){
                                currentLocaleId = i;}
                            i++;
                            }
                        this.SupportedLocalesOptionButton.Selected = currentLocaleId;
                    }
                }
            }
            if(child is VBoxContainer && child.Name.ToLower().Contains("manage")){
                this.ManageGameDataVboxContainer = (VBoxContainer)child;
                foreach(Node child2 in child.GetChildren()){
                    if(child2.Name.ToLower().Contains("changegameslot")){
                        this.ChangeGameSlotButton = (HoverableTouchScreenButton)child2.GetChildren()[0];
                        ((Label)child2).Text = (String)Strings.UI["ChangeGameSlot"];
                    } else if(child2.Name.ToLower().Contains("activegameslot")){
                        this.ActiveGameSlot = (Label)child2;
                        this.ActiveGameSlot.Text = (String)Strings.UI["GameSlot"] + " " + DbHandler.Globals.ActiveGameSlotNum;
                    } else if(child2.Name.ToLower().Contains("clear")){
                        this.ClearGameDataButton = (HoverableTouchScreenButton)child2.GetChild(0);
                        ((Label)child2).Text = (String)Strings.UI["ClearGameData"];}
                    else if(child2.Name.ToLower().Contains("back")){
                        this.BackToSettings2 = (HoverableTouchScreenButton)child2.GetChild(0);
                    }
                }
            }            
            }
        this.hideAllContainers();}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void ReactStateless(float delta){}
    public override void UpdateState(float delta){}
    public override void ReactToState(float delta){
        switch(this.ActiveState){
            case TitleScreenState.AWAITING_PRESS_START:
                this.MainMenuVBoxContainer.Visible = false;
                this.SettingsVboxContainer.Visible = false;
                if(Input.IsActionJustPressed("ui_accept") || Input.IsMouseButtonPressed(1)){
                    this.ResetActiveState(TitleScreenState.TRANSITIONING_TO_MAIN_MENU);}
            break;
            case TitleScreenState.TRANSITIONING_TO_MAIN_MENU:
                //fade to more neutral background
                var effectiveTransparency = this.Background1.GetModulate()[3];
                effectiveTransparency -= (delta / transitionDurationS);
                this.Background1.SetModulate(new Color(1,1,1, effectiveTransparency));

                //Move the logo over
                this.Logo.SetGlobalPosition(new Vector2(this.Logo.GetGlobalPosition().x + numPixelsToMoveRight * (delta / transitionDurationS),
                                                        this.Logo.GetGlobalPosition().y));

                this.transitionCounter += delta;
                if(this.transitionCounter > this.transitionDurationS){
                    this.Background1.Visible = false;
                   this.PressStartLabel.Visible = false;
                    this.ResetActiveState(TitleScreenState.IN_MAIN_MENU);
                    this.transitionCounter = 0f;
                    }
                break;
            case TitleScreenState.IN_MAIN_MENU:
                this.hideAllContainers(but: MainMenuVBoxContainer);
                if(this.PlayButton.UserHasJustSelected()){
                    SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0), 
                                        ToSceneStr: "res://scenes/level_select/level_select.tscn",
                                        effect: SceneTransitionEffect.FADE_BLACK,
                                        numSeconds: 2f,
                                        FadeOutAudio: true);}
                if(this.ExitButton.UserHasJustSelected()){
                    SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0),
                                        ToSceneStr: "res://scenes/misc/exit_black.tscn",
                                        effect: SceneTransitionEffect.FADE_INTO,
                                        numSeconds: 2f,
                                        FadeOutAudio: true);}
                if(this.SettingsButton.UserHasJustSelected()){
                    this.ResetActiveState(TitleScreenState.IN_SETTINGS);
                }
                break;
            case TitleScreenState.IN_SETTINGS:
                this.hideAllContainers(but: this.SettingsVboxContainer);
                if(this.ManageGameDataButton.UserHasJustSelected()){
                    this.ResetActiveState(TitleScreenState.IN_MANAGE_GAME_DATA);}
                if(this.BackToMainMenuButton1.UserHasJustSelected()){
                    this.ResetActiveState(TitleScreenState.IN_MAIN_MENU);}
                if(this.ChangeLanguageButton.UserHasJustSelected()){
                    this.ResetActiveState(TitleScreenState.IN_CHANGE_LANGUAGE);}
                break;
            case TitleScreenState.IN_CHANGE_LANGUAGE:
                this.hideAllContainers(but: this.ChangeLanguageContainer);
                if(this.BackToSettings1.UserHasJustSelected()){
                    this.ResetActiveState(TitleScreenState.IN_SETTINGS);
                    var globals = DbHandler.Globals;
                    globals.Locale = this.SupportedLocalesOptionButton.Text;
                    DbHandler.Globals = globals;
                    Strings.ClearCache();
                    this._Ready();
                    }
                break;
            case TitleScreenState.IN_MANAGE_GAME_DATA:
                 this.hideAllContainers(but: this.ManageGameDataVboxContainer);
                if(this.ChangeGameSlotButton.UserHasJustSelected()){
                }
                if(this.ClearGameDataButton.UserHasJustSelected()){
                    DbHandler.DeleteGlobalsAndActiveGameSlotDbs();
                    Strings.ClearCache();
                    this._Ready();
                    this.ResetActiveState(TitleScreenState.IN_MAIN_MENU);}
                if(this.BackToSettings2.UserHasJustSelected()){
                    this.ResetActiveState(TitleScreenState.IN_SETTINGS);
                }
                break;
            }}

    private void hideAllContainers(VBoxContainer but = null){
        var containers = new VBoxContainer[]{
            this.MainMenuVBoxContainer,
            this.SettingsVboxContainer,
            this.ChangeLanguageContainer,
            this.ManageGameDataVboxContainer
        };
        foreach(var container in containers){
            if(container.Equals(but)){
                container.Visible = true;
            } else {
                container.Visible = false;
            }
        }
    }
}
