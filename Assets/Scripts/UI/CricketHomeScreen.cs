using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>Interactive home dashboard for REAL CRICKET 3D.</summary>
public sealed class CricketHomeScreen : MonoBehaviour
{
    const string HomeName="RealCricketHomeScreen", SetupName="MatchSetupMenu", ResultName="PostMatchResultPanel";
    static readonly Color Navy=new Color(.008f,.009f,.012f,1), Blue=new Color(.055f,.058f,.065f,1), Gold=new Color(.83f,.69f,.22f,1), Cyan=new Color(1f,.84f,.46f,1);
    static Font UiFont=>Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    GameObject[] hud; GameObject setup,result,home; CricketMatchSetupController matchSetup;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap(){Canvas c=Object.FindObjectOfType<Canvas>();if(c==null||c.name!="GameHUD"||c.transform.Find(HomeName)!=null)return;c.gameObject.AddComponent<CricketHomeScreen>().Build(c);}

    void Build(Canvas canvas)
    {
        hud=new GameObject[canvas.transform.childCount];for(int i=0;i<hud.Length;i++){hud[i]=canvas.transform.GetChild(i).gameObject;hud[i].SetActive(false);}
        setup=canvas.transform.Find(SetupName)?.gameObject;result=canvas.transform.Find(ResultName)?.gameObject;matchSetup=Object.FindObjectOfType<CricketMatchSetupController>();
        if(setup!=null)setup.SetActive(false);if(result!=null)result.SetActive(false);
        home=Panel(canvas.transform,HomeName,Navy,new Vector2(.5f,.5f),Vector2.one,Vector2.zero);home.transform.SetAsLastSibling();
        Panel(home.transform,"GoldTopLine",Gold,new Vector2(.5f,.995f),new Vector2(1,.01f),Vector2.zero);
        Panel(home.transform,"PitchGlow",new Color(.02f,.28f,.25f,1),new Vector2(.5f,.145f),new Vector2(1,.012f),Vector2.zero);
        Image emblem=Picture(home.transform,"REAL_CRICKET_3D_Logo",new Vector2(.075f,.88f),new Vector2(128,128),RealCricketLogo.CreateSprite());emblem.preserveAspect=true;
        Label(home.transform,"REAL CRICKET",new Vector2(.205f,.91f),new Vector2(390,50),34,Color.white,TextAnchor.MiddleLeft);
        Label(home.transform,"3D  •  CRICKET BEYOND LIMITS",new Vector2(.205f,.865f),new Vector2(430,40),21,Gold,TextAnchor.MiddleLeft);
        Label(home.transform,"REAL CRICKET 3D",new Vector2(.52f,.89f),new Vector2(560,72),48,Color.white,TextAnchor.MiddleCenter);
        Label(home.transform,"PLAY  •  COMPETE  •  WIN",new Vector2(.52f,.835f),new Vector2(500,36),22,Gold,TextAnchor.MiddleCenter);
        Pill(home.transform,"OFFLINE MATCH MODE",new Vector2(.82f,.92f),new Vector2(250,48));
        Pill(home.transform,"BUILD  0.1.0",new Vector2(.94f,.86f),new Vector2(220,44));
        Label(home.transform,"CHOOSE YOUR GAME",new Vector2(.205f,.765f),new Vector2(430,42),25,Cyan,TextAnchor.MiddleLeft);
        string[] titles={"QUICK MATCH","TOURNAMENTS","RCPL LEAGUE","CAREER MODE","TEST MATCH","MULTIPLAYER"};
        string[] subs={"Play instantly","4-team knockout","Coming soon","Coming soon","Five-day challenge","Coming soon"};
        Color[] colors={Gold,new Color(.68f,.52f,.16f,1),new Color(.76f,.62f,.27f,1),new Color(.48f,.37f,.13f,1),Cyan,new Color(.62f,.49f,.18f,1)};
        float[] xx={.145f,.345f,.545f}, yy={.575f,.345f};
        for(int i=0;i<6;i++){UnityAction action=i==1?(UnityAction)Tournament:(i==4?TestMatch:(i==0?Play:null));Card(home.transform,titles[i],subs[i],new Vector2(xx[i%3],yy[i/3]),colors[i],MakeTile(i),action);}
        GameObject hero=Panel(home.transform,"SeasonFeature",new Color(.025f,.09f,.18f,1),new Vector2(.81f,.49f),new Vector2(.30f,.55f),Vector2.zero);
        Panel(hero.transform,"GoldRule",Gold,new Vector2(.5f,.985f),new Vector2(1,.012f),Vector2.zero);
        Image batter=Picture(hero.transform,"OriginalBatterArt",new Vector2(.5f,.65f),new Vector2(350,380),MakeBatter());batter.preserveAspect=true;
        Label(hero.transform,"SEASON 01",new Vector2(.5f,.29f),new Vector2(400,42),24,Gold,TextAnchor.MiddleCenter);
        Label(hero.transform,"CHAMPIONS CUP",new Vector2(.5f,.22f),new Vector2(450,60),34,Color.white,TextAnchor.MiddleCenter);
        Label(hero.transform,"YOUR NEXT CRICKET STORY STARTS HERE",new Vector2(.5f,.15f),new Vector2(470,38),16,Color.white,TextAnchor.MiddleCenter);
        ActionButton(hero.transform,"PLAY NOW",new Vector2(.5f,.075f),new Vector2(310,58),Gold,Play);
        string[] offers={"★  DAILY REWARDS","♛  SUBSCRIPTION","✦  SUPER OFFERS","◆  STORE"};
        for(int i=0;i<4;i++){ActionButton(home.transform,offers[i],new Vector2(.965f,.70f-i*.125f),new Vector2(190,76),Blue,null);}
        string[] nav={"HOME","MY TEAM","STATISTICS","STORE","SETTINGS"};float[] nx={.12f,.31f,.50f,.69f,.88f};
        for(int i=0;i<nav.Length;i++){ActionButton(home.transform,nav[i],new Vector2(nx[i],.075f),new Vector2(290,72),i==0?Gold:Blue,i==0?(UnityAction)(()=>{}):null);}
        Label(home.transform,"REAL CRICKET STUDIOS  •  CRICKET BEYOND LIMITS",new Vector2(.5f,.018f),new Vector2(900,26),15,new Color(.6f,.72f,.82f),TextAnchor.MiddleCenter);
    }

    void Play(){home.SetActive(false);Reveal();if(setup!=null)setup.SetActive(true);else if(matchSetup!=null)matchSetup.StartConfiguredMatch();}
    void Tournament(){home.SetActive(false);Reveal();if(matchSetup!=null&&matchSetup.CanStartTournament)matchSetup.CreateFourTeamTournamentAndStart();else if(setup!=null)setup.SetActive(true);}
    void TestMatch(){Play();if(matchSetup!=null)matchSetup.SelectTest();}
    void Reveal(){foreach(GameObject g in hud)if(g!=null&&g!=setup&&g!=result)g.SetActive(true);if(result!=null)result.SetActive(false);}

    static GameObject Panel(Transform p,string name,Color color,Vector2 a,Vector2 span,Vector2 size)
    {GameObject g=new GameObject(name,typeof(RectTransform),typeof(Image));g.transform.SetParent(p,false);RectTransform r=g.GetComponent<RectTransform>();r.anchorMin=a-span*.5f;r.anchorMax=a+span*.5f;r.sizeDelta=size;g.GetComponent<Image>().color=color;return g;}
    static Image Picture(Transform p,string name,Vector2 a,Vector2 size,Sprite s)
    {GameObject g=new GameObject(name,typeof(RectTransform),typeof(Image));g.transform.SetParent(p,false);RectTransform r=g.GetComponent<RectTransform>();r.anchorMin=a;r.anchorMax=a;r.sizeDelta=size;Image im=g.GetComponent<Image>();im.sprite=s;im.raycastTarget=false;return im;}
    static Text Label(Transform p,string value,Vector2 a,Vector2 size,int font,Color color,TextAnchor align)
    {GameObject g=new GameObject("Text_"+value.Replace(" ",""),typeof(RectTransform),typeof(Text));g.transform.SetParent(p,false);RectTransform r=g.GetComponent<RectTransform>();r.anchorMin=a;r.anchorMax=a;r.sizeDelta=size;Text t=g.GetComponent<Text>();t.text=value;t.font=UiFont;t.fontSize=font;t.fontStyle=FontStyle.Bold;t.alignment=align;t.color=color;t.raycastTarget=false;return t;}
    static void Pill(Transform p,string s,Vector2 a,Vector2 size){GameObject g=Panel(p,"StatusPill",Blue,a,Vector2.zero,size);Label(g.transform,s,new Vector2(.5f,.5f),size,18,Color.white,TextAnchor.MiddleCenter);}
    static void ActionButton(Transform p,string s,Vector2 a,Vector2 size,Color c,UnityAction action)
    {GameObject g=Panel(p,"Button_"+s.Replace(" ",""),c,a,Vector2.zero,size);RectTransform r=g.GetComponent<RectTransform>();r.anchorMin=a;r.anchorMax=a;r.sizeDelta=size;Button b=g.AddComponent<Button>();b.targetGraphic=g.GetComponent<Image>();if(action==null)b.interactable=false;else b.onClick.AddListener(action);Label(g.transform,s,new Vector2(.5f,.5f),size,Mathf.Clamp((int)(size.y*.27f),16,27),Color.white,TextAnchor.MiddleCenter);}
    static void Card(Transform p,string title,string sub,Vector2 a,Color accent,Sprite art,UnityAction action)
    {GameObject g=Panel(p,"Mode_"+title.Replace(" ",""),new Color(.025f,.075f,.14f,1),a,Vector2.zero,new Vector2(350,205));RectTransform r=g.GetComponent<RectTransform>();r.anchorMin=a;r.anchorMax=a;r.sizeDelta=new Vector2(350,205);Panel(g.transform,"Accent",accent,new Vector2(.5f,.98f),new Vector2(1,.025f),Vector2.zero);Image im=Picture(g.transform,"OriginalArtwork",new Vector2(.78f,.5f),new Vector2(142,142),art);im.preserveAspect=true;Label(g.transform,title,new Vector2(.38f,.68f),new Vector2(240,44),23,Color.white,TextAnchor.MiddleLeft);Label(g.transform,sub,new Vector2(.38f,.40f),new Vector2(240,38),18,accent,TextAnchor.MiddleLeft);Button b=g.AddComponent<Button>();b.targetGraphic=g.GetComponent<Image>();if(action==null)b.interactable=false;else b.onClick.AddListener(action);}
    static Sprite MakeTile(int type){const int n=160;Texture2D t=new Texture2D(n,n,TextureFormat.RGBA32,false);Color clear=new Color(0,0,0,0),gold=Gold,red=new Color(.83f,.69f,.22f,1),cyan=Cyan;for(int y=0;y<n;y++)for(int x=0;x<n;x++){float dx=x-80,dy=y-80,r=Mathf.Sqrt(dx*dx+dy*dy);Color c=clear;if(type==0&&r<54)c=red;if(type==0&&r>49&&r<54)c=Color.white;if(type==1&&r<60&&y>50)c=gold;if(type==2&&r<52&&y<120)c=cyan;if(type==3&&x>42&&x<118&&y>28&&y<130)c=cyan;if(type==4&&((x>52&&x<62)||(x>98&&x<108))&&y>25&&y<135)c=gold;if(type==5&&r<25)c=cyan;t.SetPixel(x,y,c);}t.Apply();return Sprite.Create(t,new Rect(0,0,n,n),new Vector2(.5f,.5f),100);}
    static Sprite MakeBatter(){const int n=256;Texture2D t=new Texture2D(n,n,TextureFormat.RGBA32,false);Color clear=new Color(0,0,0,0),skin=new Color(.75f,.46f,.3f,1),kit=new Color(1,.28f,.025f,1),bat=new Color(.9f,.7f,.4f,1),dark=new Color(.015f,.04f,.09f,1);for(int y=0;y<n;y++)for(int x=0;x<n;x++){Color c=clear;float h=Mathf.Sqrt((x-130)*(x-130)+(y-205)*(y-205));if(h<29)c=skin;if(x>88&&x<176&&y>95&&y<176)c=kit;if(x>60&&x<120&&y>140&&y<165)c=skin;if(x>155&&x<205&&y>145&&y<165)c=skin;if(x>185&&x<210&&y>40&&y<230)c=bat;if(x>105&&x<165&&y>25&&y<90)c=dark;t.SetPixel(x,y,c);}t.Apply();return Sprite.Create(t,new Rect(0,0,n,n),new Vector2(.5f,.5f),100);}
}