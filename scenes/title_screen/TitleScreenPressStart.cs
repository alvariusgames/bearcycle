using Godot;
using System;
using System.Collections.Generic;

public enum TitleScreenState { AWAITING_PRESS_START, TRANSITIONING_TO_MAIN_MENU, 
                               IN_MAIN_MENU, IN_SLOTS, SLOT_DELETE_1_PROMPT, SLOT_DELETE_2_PROMPT,
                               IN_SETTINGS, IN_CHANGE_LANGUAGE, IN_CONTROL_SETTINGS}
public class TitleScreenPressStart : FSMNode2D<TitleScreenState>
{

    public override TitleScreenState InitialState { get { return TitleScreenState.AWAITING_PRESS_START;}set{}}
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

    private VBoxContainer SlotsVBoxContainer;
    private SlotArea SlotArea1;
    private SlotArea SlotArea2;
    private SlotArea SlotArea3;
    private HoverableTouchScreenButton BackToMainMenuButton2;
    private Sprite TranslucentPopupBackground;
    private Popup1 SlotDelete1Prompt;
    private Popup1 SlotDelete2Prompt;

    private VBoxContainer SettingsVboxContainer;
    private HoverableTouchScreenButton ManageGameDataButton;
    private HoverableTouchScreenButton ChangeLanguageButton;
    private HoverableTouchScreenButton BackToMainMenuButton1;

    private VBoxContainer ControlVboxContainer;
    private HoverableTouchScreenButton ClearGameDataButton;
    private HoverableTouchScreenButton ChangeGameSlotButton;
    private Label ActiveGameSlot;
    private HoverableTouchScreenButton BackToSettings2;
    private RemapActionHoverableTouchScreenButton attackRemapButton;
    private RemapActionHoverableTouchScreenButton forageRemapButton;
    private RemapActionHoverableTouchScreenButton itemRemapButton;
    private HoverableTouchScreenButton ResetToDefaults;

    private VBoxContainer ChangeLanguageContainer;
    private ScrollableOptionButton SupportedLocalesOptionButton;
    private Dictionary<string, string> LocaleNameToLocale = new Dictionary<string, string>();
    private HoverableTouchScreenButton BackToSettings1;


    private Label PressStartLabel;
    private Label VersionLabel;

    private float transitionCounter = 0f;
    private float transitionDurationS = 0.75f;

    private const float logoRightSideOfScreenPercentagePos = 0.3f;
    private float numPixelsToMoveRight = (OS.GetScreenSize().x * (0.5f - logoRightSideOfScreenPercentagePos));


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
                    this.Logo = (Sprite)child;}
                else if(child.Name.ToLower().Contains("translucentpopupbackground")){
                    this.TranslucentPopupBackground = (Sprite)child;
                    this.TranslucentPopupBackground.Visible = false;
                    foreach(Node child2 in child.GetChildren()){
                        if(child2 is Popup1 && child2.Name.ToLower().Contains("1")){
                            this.SlotDelete1Prompt = (Popup1)child2;
                            this.SlotDelete1Prompt.Visible = false;}
                        if(child2 is Popup1 && child2.Name.ToLower().Contains("2")){
                            this.SlotDelete2Prompt = (Popup1)child2;
                            this.SlotDelete2Prompt.Visible = false;}}
                }}
            if(child is Label){
                if(child.Name.ToLower().Contains("start")){
                    this.PressStartLabel = (Label)child;}
                if(child.Name.ToLower().Contains("version")){
                    this.VersionLabel = (Label)child;
                    this.VersionLabel.Text = main.VERSION;}}
            if(child is VBoxContainer && child.Name.ToLower().Contains("mainmenu")){
                this.MainMenuVBoxContainer = (VBoxContainer)child;
                foreach(Node child2 in child.GetChildren()){
                    if(child2.Name.ToLower().Contains("play")){
                        var playLabel = (Label)child2;
                        this.PlayButton = (HoverableTouchScreenButton)playLabel.GetChild(0);}
                    if(child2.Name.ToLower().Contains("exit")){
                        var exitLabel = (Label)child2;
                        this.ExitButton = (HoverableTouchScreenButton)exitLabel.GetChild(0);}
                    if(child2.Name.ToLower().Contains("settings")){
                        var settingsLabel = (Label)child2;
                        this.SettingsButton = (HoverableTouchScreenButton)settingsLabel.GetChild(0);}
                    }}
            if(child is VBoxContainer && child.Name.ToLower().Contains("slot")){
                this.SlotsVBoxContainer = (VBoxContainer)child;
                foreach(Node child2 in child.GetChildren()){
                    if(child2.Name.ToLower().Contains("mainmenu")){
                        this.BackToMainMenuButton2 = (HoverableTouchScreenButton)child2.GetChild(0);}
                    if(child2.GetChild(0) is SlotArea && child2.Name.Contains("1")){
                        this.SlotArea1 = (SlotArea)child2.GetChild(0);}
                    if(child2.GetChild(0) is SlotArea && child2.Name.Contains("2")){
                        this.SlotArea2 = (SlotArea)child2.GetChild(0);}
                    if(child2.GetChild(0) is SlotArea && child2.Name.Contains("3")){
                        this.SlotArea3 = (SlotArea)child2.GetChild(0);}
                }}
            if(child is VBoxContainer && child.Name.ToLower().Contains("settings")){
                this.SettingsVboxContainer = (VBoxContainer)child;
                foreach(Node child2 in child.GetChildren()){
                    if(child2.Name.ToLower().Contains("manage")){
                        var manageLabel = (Label)child2;
                        this.ManageGameDataButton = (HoverableTouchScreenButton)child2.GetChild(0);}
                    if(child2.Name.ToLower().Contains("mainmenu")){
                        this.BackToMainMenuButton1 = (HoverableTouchScreenButton)child2.GetChild(0);}
                    if(child2.Name.ToLower().Contains("language")){
                        var langLabel = (Label)child2;
                        this.ChangeLanguageButton = (HoverableTouchScreenButton)child2.GetChild(0);
                    }}}
            if(child is VBoxContainer && child.Name.ToLower().Contains("language")){
                this.ChangeLanguageContainer = (VBoxContainer)child;
                foreach(Node child2 in child.GetChildren()){
                    if(child2.Name.ToLower().Contains("mainmenu")){
                        this.BackToSettings1 = (HoverableTouchScreenButton)child2.GetChildren()[0];
                    }
                    if(child2.Name.ToLower().Contains("language")){
                        this.SupportedLocalesOptionButton = (ScrollableOptionButton)child2;
                        this.SupportedLocalesOptionButton.Theme = new Theme();
                        this.SupportedLocalesOptionButton.Theme.DefaultFont = new DynamicFont();
                        ((DynamicFont)this.SupportedLocalesOptionButton.Theme.DefaultFont).FontData = (DynamicFontData)GD.Load("res://media/fonts/no_locale_en.ttf");
                        if(main.PlatformType.Equals(PlatformType.MOBILE)){
                             this.SupportedLocalesOptionButton.RectScale = new Vector2(1.5f,1.5f);
                             ((DynamicFont)this.SupportedLocalesOptionButton.Theme.DefaultFont).Size = 72;}
                        else{
                            ((DynamicFont)this.SupportedLocalesOptionButton.Theme.DefaultFont).Size = 48;}
                        this.SupportedLocalesOptionButton.Clear();
                        int id = 0;
                        int currentLocaleId = 0;
                        var uniqueLocales = new HashSet<String>();
                        foreach(var locale in TranslationServer.GetLoadedLocales()){
                            uniqueLocales.Add((String)locale);}
                        foreach(var locale in uniqueLocales){
                            var localeName = TranslationServer.GetLocaleName(locale);
                            this.LocaleNameToLocale[TranslationServer.GetLocaleName(locale)] = locale;
                            this.SupportedLocalesOptionButton.AddItem(localeName, id);
                            if(locale == TranslationServer.GetLocale()){
                                currentLocaleId = id;}
                            id++;}
                        this.SupportedLocalesOptionButton.Selected = currentLocaleId;}}}
            if(child is VBoxContainer && child.Name.ToLower().Contains("control")){
                this.ControlVboxContainer = (VBoxContainer)child;
                foreach(Node child2 in child.GetChildren()){
                    if(child2.Name.ToLower().Contains("back")){
                        this.BackToSettings2 = (HoverableTouchScreenButton)child2.GetChild(0);}
                    if(child2.Name.ToLower().Contains("attack")){
                        this.attackRemapButton = (RemapActionHoverableTouchScreenButton)child2.GetChild(0);}
                    if(child2.Name.ToLower().Contains("forage")){
                        this.forageRemapButton = (RemapActionHoverableTouchScreenButton)child2.GetChild(0);}
                    if(child2.Name.ToLower().Contains("item")){
                        this.itemRemapButton = (RemapActionHoverableTouchScreenButton)child2.GetChild(0);}
                    if(child2.Name.ToLower().Contains("default")){
                        this.ResetToDefaults = (HoverableTouchScreenButton)child2.GetChild(0);}}}}
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
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                        "res://media/samples/ui/accept_1.wav");
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
                                FadeOutAudio: true);
                    return;
                    this.SlotArea1.SetToUnhovered();
                    this.SlotArea2.SetToUnhovered();
                    this.SlotArea3.SetToUnhovered();
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/accept_1.wav");
                    this.ResetActiveState(TitleScreenState.IN_SLOTS);}
                if(this.ExitButton.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                        new string[]{"res://media/samples/ui/decline_1.wav"});
                    SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0),
                                                 ToSceneStr: "res://scenes/misc/exit_black.tscn",
                                                 effect: SceneTransitionEffect.FADE_INTO,
                                                 numSeconds: 2f,
                                                 FadeOutAudio: true);}
                if(this.SettingsButton.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                        new string[]{"res://media/samples/ui/accept_1.wav"});
                    this.ResetActiveState(TitleScreenState.IN_SETTINGS);
                }
                break;
            case TitleScreenState.IN_SLOTS:
                this.hideAllContainers(but: SlotsVBoxContainer);
                var anyPlaySelected = false;
                var anyDeleteSelected = false;
                if(this.SlotArea1.PlaySlotButton.UserHasJustSelected()){
                    var globals = DbHandler.Globals;
                    globals.ActiveGameSlotNum = 1;
                    DbHandler.Globals = globals;
                    anyPlaySelected = true;}
                if(this.SlotArea1.DeleteSlotButton.UserHasJustSelected()){
                    var globals = DbHandler.Globals;
                    globals.ActiveGameSlotNum = 1;
                    DbHandler.Globals = globals; 
                    anyDeleteSelected = true;}
                if(this.SlotArea2.PlaySlotButton.UserHasJustSelected()){
                    var globals = DbHandler.Globals;
                    globals.ActiveGameSlotNum = 2;
                    DbHandler.Globals = globals; 
                    anyPlaySelected = true;}
                if(this.SlotArea2.DeleteSlotButton.UserHasJustSelected()){
                    var globals = DbHandler.Globals;
                    globals.ActiveGameSlotNum = 2;
                    DbHandler.Globals = globals; 
                    anyDeleteSelected = true;}
                if(this.SlotArea3.PlaySlotButton.UserHasJustSelected()){
                    var globals = DbHandler.Globals;
                    globals.ActiveGameSlotNum = 3;
                    DbHandler.Globals = globals; 
                    anyPlaySelected = true;}
                if(this.SlotArea3.DeleteSlotButton.UserHasJustSelected()){
                    var globals = DbHandler.Globals;
                    globals.ActiveGameSlotNum = 3;
                    DbHandler.Globals = globals; 
                    anyDeleteSelected = true;}

                if(anyPlaySelected){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/accept_1.wav");
                    SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0), 
                                                 ToSceneStr: "res://scenes/level_select/level_select.tscn",
                                                 effect: SceneTransitionEffect.FADE_BLACK,
                                                 numSeconds: 2f,
                                                 FadeOutAudio: true);}
                if(anyDeleteSelected){
                    this.SlotArea1.SetToUnhovered();
                    this.SlotArea2.SetToUnhovered();
                    this.SlotArea3.SetToUnhovered();
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/accept_1.wav");
                    this.ResetActiveState(TitleScreenState.SLOT_DELETE_1_PROMPT);}
                if(this.BackToMainMenuButton2.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                        "res://media/samples/ui/decline_1.wav");
                    this.ResetActiveState(TitleScreenState.IN_MAIN_MENU);} 
                break;
            case TitleScreenState.SLOT_DELETE_1_PROMPT:
                this.hideAllContainers();
                this.SlotArea1.SetToUnhovered();
                this.SlotArea2.SetToUnhovered();
                this.SlotArea3.SetToUnhovered();
                this.TranslucentPopupBackground.Visible = true;
                this.SlotDelete1Prompt.Visible = true;
                this.SlotDelete2Prompt.Visible = false;
                if(this.SlotDelete1Prompt.YesButton.UserHasJustSelected()){
                    this.SlotDelete1Prompt.Visible = false;
                    this.SlotDelete2Prompt.Visible = true;
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/accept_1.wav");
                    this.ResetActiveState(TitleScreenState.SLOT_DELETE_2_PROMPT);}
                if(this.SlotDelete1Prompt.NoButton.UserHasJustSelected()){
                    this.TranslucentPopupBackground.Visible = false;
                    this.SlotDelete1Prompt.Visible = false;
                    this.SlotDelete2Prompt.Visible = false; 
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/decline_1.wav");
                    this.ResetActiveState(TitleScreenState.IN_SLOTS);}
                break;
            case TitleScreenState.SLOT_DELETE_2_PROMPT:
                this.hideAllContainers();
                this.SlotArea1.SetToUnhovered();
                this.SlotArea2.SetToUnhovered();
                this.SlotArea3.SetToUnhovered();
                this.TranslucentPopupBackground.Visible = true;
                this.SlotDelete1Prompt.Visible = false;
                this.SlotDelete2Prompt.Visible = true;
                if(this.SlotDelete2Prompt.YesButton.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/accept_1.wav");
                    this.TranslucentPopupBackground.Visible = false;
                    this.SlotDelete1Prompt.Visible = false;
                    this.SlotDelete2Prompt.Visible = false;
                    DbHandler.DeleteActiveGameSlotDatabase();
                    this.SlotArea1._Ready();
                    this.SlotArea2._Ready();
                    this.SlotArea3._Ready();
                    this.ResetActiveState(TitleScreenState.IN_MAIN_MENU);}
                if(this.SlotDelete2Prompt.NoButton.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this, "res://media/samples/ui/decline_1.wav");
                    this.TranslucentPopupBackground.Visible = false;
                    this.SlotDelete1Prompt.Visible = false;
                    this.SlotDelete2Prompt.Visible = false; 
                    this.ResetActiveState(TitleScreenState.IN_SLOTS);}
                break;
 
                break;
            case TitleScreenState.IN_SETTINGS:
                this.hideAllContainers(but: this.SettingsVboxContainer);
                if(this.ManageGameDataButton.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                        new string[]{"res://media/samples/ui/accept_1.wav"});
                    this.ResetActiveState(TitleScreenState.IN_CONTROL_SETTINGS);}
                if(this.BackToMainMenuButton1.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                        new string[]{"res://media/samples/ui/decline_1.wav"});
                    this.ResetActiveState(TitleScreenState.IN_MAIN_MENU);}
                if(this.ChangeLanguageButton.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                        new string[]{"res://media/samples/ui/accept_1.wav"});
                    this.ResetActiveState(TitleScreenState.IN_CHANGE_LANGUAGE);}
                break;
            case TitleScreenState.IN_CHANGE_LANGUAGE:
                this.hideAllContainers(but: this.ChangeLanguageContainer);
                if(this.SupportedLocalesOptionButton.PopupIsVisible){
                    this.BackToSettings1.Visible = false;
                } else {
                    this.BackToSettings1.Visible = true;
                }
                if(this.BackToSettings1.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                        new string[]{"res://media/samples/ui/decline_1.wav"});
                    var selectedLocale = this.LocaleNameToLocale[this.SupportedLocalesOptionButton.Text];
                    if(selectedLocale != TranslationServer.GetLocale()){
                        var globals = DbHandler.Globals;
                        globals.Locale = this.LocaleNameToLocale[this.SupportedLocalesOptionButton.Text];
                        TranslationServer.SetLocale(globals.Locale);
                        DbHandler.Globals = globals;
                        SceneTransitioner.Transition(FromScene: this.GetTree().GetRoot().GetChild(0), 
                                                     ToSceneStr: "res://scenes/misc/reload_locales_workaround.tscn",
                                                     effect: SceneTransitionEffect.FADE_BLACK,
                                                     numSeconds: 1f,
                                                     FadeOutAudio: true);
                        }
                    else{
                        this.ResetActiveState(TitleScreenState.IN_SETTINGS);
                        this._Ready();}
                }
                break;
            case TitleScreenState.IN_CONTROL_SETTINGS:
                this.hideAllContainers(but: this.ControlVboxContainer);
                if(this.BackToSettings2.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                        new string[]{"res://media/samples/ui/decline_1.wav"});
                    this.ResetActiveState(TitleScreenState.IN_SETTINGS);}
                if(this.ResetToDefaults.UserHasJustSelected()){
                    SoundHandler.PlaySample<MyAudioStreamPlayer>(this,
                        new string[]{"res://media/samples/ui/accept_1.wav"});
                    DbHandler.ResetInputMapToDefault();
                    this.attackRemapButton._Ready();
                    this.forageRemapButton._Ready();
                    this.itemRemapButton._Ready();
                }
                break;
            }}

    private void hideAllContainers(VBoxContainer but = null){
        var containers = new VBoxContainer[]{
            this.MainMenuVBoxContainer,
            this.SlotsVBoxContainer,
            this.SettingsVboxContainer,
            this.ChangeLanguageContainer,
            this.ControlVboxContainer
        };
        foreach(var container in containers){
            try{
            if(container.Equals(but)){
                container.Visible = true;
            } else {
                container.Visible = false;
            }
            } catch{}
        }
    }
}
