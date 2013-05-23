using UnityEngine;
using System;
using System.Collections;

public class PlayerHUD : MonoBehaviour
{
	const int COUNTDOWN_START = 3;	
	const int MAX_COUNTDOWN_SIZE = 60;	
	const int ROUND_TIMER_WIDTH = 200;
	
	int DefaultWidth = 1024;
	
	int DefaultTexWidth  = 50;
	int DefaultTexHeight  = 50;
	
	float TextureWidth = 0;
	float TextureHeight = 0;
	
	float GUIscale = 1;
	
	public Texture2D[] textureCarridges = new Texture2D[3];
	public Texture2D textureWeaponShotgun;
	
	bool m_menu;	
	float m_menuBoxSize;	
	float m_menuPaddingTop;	
		
	void Awake()
	{
		// Disable cursor visibility
		Screen.showCursor = false;
		
		m_menu = false;
		m_menuBoxSize = 200;
		m_menuPaddingTop = 50;
	}
	
	void CalculateScale()
	{
		GUIscale = (float)Screen.width / DefaultWidth;
		TextureWidth = DefaultTexWidth * GUIscale;
		TextureHeight = DefaultTexHeight * GUIscale;
	}
	
	void DrawCarriages()
	{		
		TrainCarriages trainCarriages = Players.Get().GetMe().Train.GetComponent<TrainCarriages>();	
		
		int iOffsetX = 1;
		
		if(trainCarriages.GetNumCarriages() < 10)
		{		
			for (int index = 1; index <= trainCarriages.GetNumCarriages(); index++)
			{				
				GUI.DrawTexture(new Rect(Screen.width - iOffsetX * TextureWidth, 0, TextureWidth, TextureHeight), textureCarridges[0], ScaleMode.ScaleToFit);
				if(index % 5 == 0)
				{
					TextureHeight += TextureHeight;
					iOffsetX = 0;
				}	
				
				iOffsetX++;
			}
		}
		//If there are more than 10 carriages, simply draw the texture with the number of carriages overlayed.
		else
		{
			Rect TextureBounds = new Rect(Screen.width - iOffsetX * TextureWidth, 0, TextureWidth, TextureHeight);					
			GUI.DrawTexture(TextureBounds, textureCarridges[0], ScaleMode.ScaleToFit);
			
			float RectX = TextureBounds.x + (TextureBounds.width / 2);
			float RectY = TextureHeight / 3;
			
			Rect NumberBounds = new Rect(RectX, RectY, RectX, TextureBounds.height / 4);
			
			GUI.color = Color.red;
			GUI.Label(NumberBounds, trainCarriages.GetNumCarriages().ToString());
			GUI.color = Color.white;
		}
	}
	
	void DrawCountdown()
	{
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.fontSize = (int) (MAX_COUNTDOWN_SIZE * (Time.timeSinceLevelLoad - Mathf.Floor(Time.timeSinceLevelLoad)));
		
		Game game = GameObject.Find("The Game").GetComponent<Game>();
		float countdown = game.RoundStartTime - Time.timeSinceLevelLoad;
		string countdownText = null;
		if (countdown >= 1 && countdown < COUNTDOWN_START + 1)
		{
			countdownText = ((int) countdown).ToString();
		}
		else if (countdown >= 0 && countdown < 1)
		{
			countdownText = "Begin!";
		}
		
		GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200), countdownText, style);
	}
	
	void DrawMenu()
	{
		if (m_menu)
		{
			float halfMenuBoxSize = m_menuBoxSize / 2.0f;
			
			GUI.Box(new Rect(Screen.width / 2 - halfMenuBoxSize, Screen.height / 2 - halfMenuBoxSize, m_menuBoxSize,
				m_menuBoxSize), "Menu");
			
			GUILayout.BeginArea(new Rect(Screen.width / 2 - halfMenuBoxSize + GUIConstants.BOX_PADDING,
				Screen.height / 2 - halfMenuBoxSize + m_menuPaddingTop, m_menuBoxSize - GUIConstants.BOX_PADDING_DOUBLE,
				m_menuBoxSize - GUIConstants.BOX_PADDING_DOUBLE - m_menuPaddingTop));
				
			if (GUILayout.Button("Continue"))
			{
				Screen.showCursor = false;
				m_menu = false;
			}
			
			if (GUILayout.Button("Leave Game"))
			{
				Session.Get().LeaveGame();
			}
			
			if (GUILayout.Button("Quit"))
			{
				Session.Get().Quit();
			}
			
			GUILayout.EndArea();
		}
	}
	
	void DrawRoundTimer()
	{
		Game game = GameObject.Find("The Game").GetComponent<Game>();
		
		GUI.Label(new Rect(GUIConstants.GAP_SIZE, GUIConstants.GAP_SIZE, ROUND_TIMER_WIDTH,
			GUIConstants.ONE_LINE_BOX_HEIGHT), "Round " + Session.Get().GetRound() + " : " +
			(int) game.RoundTimeRemaining);
	}
	
	void DrawWeapons()
	{
		GUI.DrawTexture(new Rect((textureWeaponShotgun.width / 2) - 100, Screen.height - (textureWeaponShotgun.height / 2),
			(textureWeaponShotgun.width / 2), (textureWeaponShotgun.height / 2)), textureWeaponShotgun, ScaleMode.ScaleAndCrop);
 	}
	
	void OnGUI()
	{
		DrawCarriages();
		DrawCountdown();
		DrawMenu();
		DrawRoundTimer();
		DrawWeapons();
	}
	
	void Start()
	{
	}
	
	void Update() 
	{
		CalculateScale();
		
		if (Input.GetKeyUp(KeyCode.Escape) && !m_menu)
		{
			m_menu = true;
			Screen.showCursor = true;
		}
		else if (Input.GetKeyUp(KeyCode.Escape) && m_menu)
		{
			m_menu = false;
			Screen.showCursor = false;	
		}
	}
}
