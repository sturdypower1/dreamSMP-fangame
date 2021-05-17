using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Scenes;
using System.Collections;

public class TitleScreenSystem : SystemBase
{
    private titleMenuSelectables currentSelection;
    private SceneSystem sceneSystem;
    private SceneSectionData test;
    private SettingsMenuSystem settingsMenuSystem;
    private bool isSelected;
    private bool isInSettings;
    private bool isLinked;


    protected override void OnStartRunning()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        base.OnStartRunning();

        currentSelection = titleMenuSelectables.Start;
        AudioManager.playSong("menuMusic");

        settingsMenuSystem = World.GetOrCreateSystem<SettingsMenuSystem>();

        InputGatheringSystem.currentInput = CurrentInput.ui;
    }

  

    protected override void OnUpdate()
    {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithAll<TitleMenuTag>()
        .WithNone<OptionMenuTag>()
        .ForEach((in UIDocument UIDoc) => {
            VisualElement root = UIDoc.rootVisualElement;
            if(root == null){
                Debug.Log("root wasn't found");
            }
            else{
                //references to the ui
                VisualElement startButton = root.Q<VisualElement>("Start");
                VisualElement optionsButton = root.Q<VisualElement>("Options");
                VisualElement creditsButton = root.Q<VisualElement>("Credits");
                VisualElement exitButton = root.Q<VisualElement>("exit");
                VisualElement titleBackground =root.Q<VisualElement>("title_background");
                VisualElement creditsBackground = root.Q<VisualElement>("credits_background");
                switch(currentSelection){
                    case(titleMenuSelectables.Start):
                        if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    InputGatheringSystem.currentInput = CurrentInput.overworld;
                                    sceneSystem.UnloadScene(SubSceneReferences.Instance.TitleSubScene.SceneGUID);
                                    sceneSystem.LoadSceneAsync(SubSceneReferences.Instance.WorldSubScene.SceneGUID);
                                    AudioManager.stopSong("menuMusic");
                                    isLinked = false;
                                    //Enabled = false;
                                }
                        else if(input.moveup){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Exit;

                                    exitButton.RemoveFromClassList("not_selected");
                                    exitButton.AddToClassList("selected");

                                    startButton.RemoveFromClassList("selected");
                                    startButton.AddToClassList("not_selected");

                                }
                        else if(input.movedown){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Options;

                                    optionsButton.RemoveFromClassList("not_selected");
                                    optionsButton.AddToClassList("selected");

                                    startButton.RemoveFromClassList("selected");
                                    startButton.AddToClassList("not_selected");
                                }
                        break;
                            case(titleMenuSelectables.Options):
                                if(isSelected){
                                    //do nothing, waiting for player to exit the settings
                                    if(!isInSettings){
                                        titleBackground.visible = true;
                                        isSelected = false;
                                        settingsMenuSystem.OnSettingsExit -= ReactivateTitle_OnSettingsExit;
                                    }
                                }
                                else if(input.goselected){
                                    AudioManager.playSound("menuchange");

                                    settingsMenuSystem.OnSettingsExit += ReactivateTitle_OnSettingsExit;

                                    titleBackground.visible = false;
                                    isInSettings = true;
                                    isSelected = true;
                                    settingsMenuSystem.ActivateMenu();
                                }
                                else if(input.moveup){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Start;

                                    startButton.RemoveFromClassList("not_selected");
                                    startButton.AddToClassList("selected");

                                    optionsButton.RemoveFromClassList("selected");
                                    optionsButton.AddToClassList("not_selected");
                                }
                                else if(input.movedown){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Credits;

                                    creditsButton.RemoveFromClassList("not_selected");
                                    creditsButton.AddToClassList("selected");

                                    optionsButton.RemoveFromClassList("selected");
                                    optionsButton.AddToClassList("not_selected");
                                }
                                break;
                            case(titleMenuSelectables.Credits):
                                if(isSelected){
                                    //make it so if the buttons are pressed they send to their according site
                                    if(!isLinked){
                                        Button technoYoutubeButton = root.Q<Button>("techno_youtube");
                                        Button technoTwitterButton = root.Q<Button>("techno_twitter");

                                        Button tommyYoutubeButton = root.Q<Button>("tommy_youtube");
                                        Button tommyTwitterButton = root.Q<Button>("tommy_twitter");
                                        Button tommyTwitchButton = root.Q<Button>("tommy_twitch");

                                        Button wilburYoutubeButton = root.Q<Button>("wilbur_youtube");
                                        Button wilburTwitterButton = root.Q<Button>("wilbur_twitter");
                                        Button wilburTwitchButton = root.Q<Button>("wilbur_twitch");
                                        technoYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTechno(websites.youtube));
                                        technoTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTechno(websites.twitter));

                                        tommyYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.youtube));
                                        tommyTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.twitter));
                                        tommyTwitchButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.twitch));

                                        wilburYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.youtube));
                                        wilburTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.twitter));
                                        wilburTwitchButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.twitch));

                                        isLinked = true;
                                    }
                                    if(input.goback){
                                        AudioManager.playSound("menuchange");
                                        creditsBackground.visible = false;
                                        titleBackground.visible = true;
                                        isSelected = false;
                                    }
                                }
                                else if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    isSelected = true;
                                    creditsBackground.visible = true;
                                    titleBackground.visible = false;

                                }
                                else if(input.moveup){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Options;

                                    optionsButton.RemoveFromClassList("not_selected");
                                    optionsButton.AddToClassList("selected");

                                    creditsButton.RemoveFromClassList("selected");
                                    creditsButton.AddToClassList("not_selected");

                                }
                                else if(input.movedown){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Exit;

                                    creditsButton.RemoveFromClassList("selected");
                                    creditsButton.AddToClassList("not_selected");

                                    exitButton.RemoveFromClassList("not_selected");
                                    exitButton.AddToClassList("selected");
                                }
                                break;
                            case(titleMenuSelectables.Exit):
                                if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    Application.Quit();
                                }
                                else if(input.moveup){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Credits;

                                    creditsButton.RemoveFromClassList("not_selected");
                                    creditsButton.AddToClassList("selected");

                                    exitButton.RemoveFromClassList("selected");
                                    exitButton.AddToClassList("not_selected");

                                }
                                /*else if(input.movedown){
                                    AudioManager.playSound("menuchange");
                                    currentSelection = titleMenuSelectables.Start;

                                    startButton.RemoveFromClassList("not_selected");
                                    startButton.AddToClassList("selected");

                                    exitButton.RemoveFromClassList("selected");
                                    exitButton.AddToClassList("not_selected");
                                }*/
                                break;


                }
            }
            }).Run();
        }
    private void ReactivateTitle_OnSettingsExit(System.Object sender, System.EventArgs e){
        isInSettings = false;
    }
}

public enum currentTitleMenu{
    MainMenu,
    OptionsMenu,
    Credits
}
public enum titleMenuSelectables{
    Start, 
    Options,
    Credits,
    Exit
}
public class temp : MonoBehaviour{
    public static void quitGame(){
        Application.Quit(0);
    }
}
