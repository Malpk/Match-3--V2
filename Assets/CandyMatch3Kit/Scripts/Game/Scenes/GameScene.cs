// Copyright (C) 2017-2018 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using GameVanilla.Core;
using GameVanilla.Game.Common;
using GameVanilla.Game.Popups;
using GameVanilla.Game.UI;

namespace GameVanilla.Game.Scenes
{
    /// <summary>
    /// This class contains the logic associated to the game scene.
    /// </summary>
	public class GameScene : BaseScene
	{
		public GameBoard gameBoard;

		public Level level;

		public FxPool fxPool;

		[SerializeField]
		private Image ingameBoosterPanel;

		[SerializeField]
		private Text ingameBoosterText;

		private bool _blockInput;

		private bool gameStarted;
		private bool gameFinished;

		private bool boosterMode;
		private BuyBoosterButton currentBoosterButton;
		private int ingameBoosterBgTweenId;

	    /// <summary>
	    /// Unity's Awake method.
	    /// </summary>
		private void Awake()
		{
			Assert.IsNotNull(gameBoard);
			Assert.IsNotNull(fxPool);
			Assert.IsNotNull(ingameBoosterPanel);
			Assert.IsNotNull(ingameBoosterText);
		}

	    /// <summary>
	    /// Unity's Start method.
	    /// </summary>
		private void Start()
		{
			gameBoard.LoadLevel();

			level = gameBoard.level;
			StartGame();
		}

	    /// <summary>
	    /// Unity's Update method.
	    /// </summary>
		private void Update()
		{
			if (!gameStarted || gameFinished)
			{
				return;
			}

            if (currentPopups.Count > 0)
            {
                return;
            }
			
			if (!_blockInput)
			{

				if (boosterMode)
				{
					if (currentBoosterButton.boosterType == BoosterType.Switch)
					{
						gameBoard.HandleSwitchBoosterInput(currentBoosterButton);
					}
					else
					{
						gameBoard.HandleBoosterInput(currentBoosterButton);
					}
				}
				else
				{
					gameBoard.HandleInput();
				}
			}
		}

		public void SetInputMode(bool mode)
		{
			_blockInput = mode;
		}

	    /// <summary>
	    /// Starts the game.
	    /// </summary>
		public void StartGame()
		{
			gameStarted = true;
		    gameBoard.StartGame();
		}

	    /// <summary>
	    /// Ends the game.
	    /// </summary>
		public void EndGame()
		{
			gameFinished = true;
		    gameBoard.EndGame();
		}

	    /// <summary>
	    /// Restarts the game.
	    /// </summary>
		public void RestartGame()
		{
		    gameStarted = false;
		    gameFinished = false;
		    gameBoard.ResetLevelData();
			level = gameBoard.level;
            //OpenPopup<LevelGoalsPopup>("Popups/LevelGoalsPopup", popup => popup.SetGoals(level.goals));
		}

        /// <summary>
        /// Continues the current game with additional moves/time.
        /// </summary>
        public void Continue()
        {
            gameFinished = false;
            gameBoard.Continue();
        }


        /// <summary>
        /// Opens the lose popup.
        /// </summary>
        public void OpenLosePopup()
        {
            PuzzleMatchManager.instance.livesSystem.RemoveLife();
            OpenPopup<LosePopup>("Popups/LosePopup", popup =>
            {
                popup.SetLevel(level.id);
            });
        }

        /// <summary>
        /// Opens the popup for buying additional moves or time.
        /// </summary>
        private void OpenNoMovesOrTimePopup()
        {
            OpenPopup<NoMovesOrTimePopup>("Popups/NoMovesOrTimePopup",
                popup => { popup.SetGameScene(this); });
        }

        /// <summary>
        /// Called when the pause button is pressed.
        /// </summary>
        public void OnPauseButtonPressed()
        {
            if (currentPopups.Count == 0)
            {
                OpenPopup<InGameSettingsPopup>("Popups/InGameSettingsPopup");
            }
            else
            {
                CloseCurrentPopup();
            }
        }

        private IEnumerator OpenNoMovesOrTimePopupAsync()
        {
            yield return new WaitForSeconds(GameplayConstants.EndGamePopupDelay);
            OpenNoMovesOrTimePopup();
        }

        /// <summary>
        /// Shows the compliment text.
        /// </summary>
		/// <param name="type">The compliment type.</param>
        public void ShowComplimentText(ComplimentType type)
        {
	        if (gameFinished)
	        {
		        return;
	        }

	        var text = fxPool.complimentTextPool.GetObject();
	        text.transform.SetParent(canvas.transform, false);
	        text.GetComponent<ComplimentText>().SetComplimentType(type);
        }

		/// <summary>
		/// Enables the booster mode in the game.
		/// </summary>
		/// <param name="button">The used booster button.</param>
		public void EnableBoosterMode(BuyBoosterButton button)
		{
			boosterMode = true;
			currentBoosterButton = button;
			FadeInInGameBoosterOverlay();
			gameBoard.OnBoosterModeEnabled();

			switch (button.boosterType)
			{
				case BoosterType.Lollipop:
					ingameBoosterText.text = "Select a tile for the lollipop:";
					break;

				case BoosterType.Bomb:
					ingameBoosterText.text = "Select a tile for the bomb:";
					break;

				case BoosterType.Switch:
					ingameBoosterText.text = "Swap two tiles:";
					break;

				case BoosterType.ColorBomb:
					ingameBoosterText.text = "Select a tile for the color bomb:";
					break;
			}
		}

		/// <summary>
		/// Disables the booster mode in the game.
		/// </summary>
		public void DisableBoosterMode()
		{
			boosterMode = false;
			FadeOutInGameBoosterOverlay();
			gameBoard.OnBoosterModeDisabled();
		}

        /// <summary>
        /// Fades in the in-game booster overlay.
        /// </summary>
        private void FadeInInGameBoosterOverlay()
        {
            var tween = LeanTween.value(ingameBoosterPanel.gameObject, 0.0f, 1.0f, 0.4f).setOnUpdate(value =>
            {
                ingameBoosterPanel.GetComponent<CanvasGroup>().alpha = value;
                ingameBoosterText.GetComponent<CanvasGroup>().alpha = value;

            });
            tween.setOnComplete(() => ingameBoosterPanel.GetComponent<CanvasGroup>().blocksRaycasts = true);
            ingameBoosterBgTweenId = tween.id;
        }

        /// <summary>
        /// Fades out the in-game booster overlay.
        /// </summary>
        private void FadeOutInGameBoosterOverlay()
        {
            LeanTween.cancel(ingameBoosterBgTweenId, false);
            var tween = LeanTween.value(ingameBoosterPanel.gameObject, 1.0f, 0.0f, 0.2f).setOnUpdate(value =>
            {
                ingameBoosterPanel.GetComponent<CanvasGroup>().alpha = value;
                ingameBoosterText.GetComponent<CanvasGroup>().alpha = value;

            });
            tween.setOnComplete(() => ingameBoosterPanel.GetComponent<CanvasGroup>().blocksRaycasts = false);
        }
	}
}
