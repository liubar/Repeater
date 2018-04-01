using System.Collections;
using Domain;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
    public class GameEngine : MonoBehaviour, IGameEngine
    {
        // Override values
        public float TotalTime;
        public float SimilarityThreshold;

        // UI elements
        public Text TimeTitle;
        public Text ScoreTitle;
        public Image PatternImage;

        public LineRenderer Line;
        public ParticleSystem TrailEffect;

        // Menu panels
        public GameObject MainMenu;
        public GameObject PauseMenu;
        public ScoreMenu ScoreMainMenu;

        // Object models
        private IPlayer _player = new Player();
        private IDrawingController _drawingController;
        private IFigureManager _figureManager;
        private Easel _easel;
        private List<IRound> _roundList = new List<IRound>();

        private int _currentRound = 0;
        private bool _isPaused = true;

        public void Awake()
        {
            InitialValues();

            IFigureComparision comparer = new RadialAlgFigureComparision();
            //IFigureComparision comparer = new DataMiningFigureComparision();
            _figureManager = new FigureManager(comparer, new RecognitionManager(TrailEffect));
            _easel = new Easel();
            _drawingController = new DrawingController(Line, TrailEffect, _easel.Board);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }

            if (_isPaused) return;

            UpdateTimer();
            _drawingController.Update();
        }

        #region Main game methods

        /// <summary>
        ///     Start game logic
        /// </summary>
        public void StartGame()
        {
            _player.Score = 0;
            TotalTime = ConfigurationValues.StartTime;
            _isPaused = false;
            Time.timeScale = 1;
            StartCoroutine(PlayRounds());

            // Enable ui elements
            TimeTitle.gameObject.SetActive(true);
            ScoreTitle.gameObject.SetActive(true);
            PatternImage.gameObject.SetActive(true);
        }

        /// <summary>
        ///     Restart logic
        /// </summary>
        public void RestartGame()
        {
            StopAllCoroutines();
            _currentRound = 0;
            StartGame();
        }

        /// <summary>
        ///     Pause logic
        /// </summary>
        public void Pause()
        {
            _isPaused = !_isPaused;
            PauseMenu.SetActive(_isPaused);
            Time.timeScale = _isPaused ? 0 : 1;
        }

        /// <summary>
        ///     End game logic
        /// </summary>
        public void EndGame()
        {
            _isPaused = true;
            Time.timeScale = 0;
            _currentRound = 0;
            StopAllCoroutines();
            ScoreTitle.text = string.Empty;

            // Disable ui elements
            TimeTitle.gameObject.SetActive(false);
            ScoreTitle.gameObject.SetActive(false);
            PatternImage.gameObject.SetActive(false);

            // Run score menu
            ScoreMainMenu.Active();
        }

        #endregion

        #region Game Logic

        /// <summary>
        ///     Game time update
        /// </summary>
        private void UpdateTimer()
        {
            TotalTime -= Time.deltaTime;
            TimeTitle.text = TotalTime.ToString("F");

            if (TotalTime < 0)
            {
                TimeTitle.text = "0.0";
                EndGame();
            }
        }

        /// <summary>
        ///     Main game logic
        /// </summary>
        /// <returns>scan time</returns>
        private IEnumerator PlayRounds()
        {
            while (true)
            {   // Start New round

                // Adding round time to the game
                TotalTime += _roundList[_currentRound].Time;
                _easel.PatternFigure = _figureManager.GetRandomFigure();
                DrawPatternFigure();
                _easel.Board.Clear();

                while (true)
                {   // fulfillment of the round
                    if (_drawingController.DrawingIsOver && !_easel.Board.IsEmpty)
                    {
                        if (_figureManager.Compare(_easel.Board, _easel.PatternFigure))
                        { // figure coincided
                            _drawingController.Clear();
                            break;
                        }
                        else
                        { // figure drawn incorrectly
                            _drawingController.Clear();
                            _easel.Board.Clear();
                        }
                    }

                    yield return new WaitForSeconds(0.1f);
                }

                // the round ended successfully
                _player.Score++;
                _currentRound++;

                ScoreTitle.text = string.Format("Score: {0}", _player.Score);
            }
        }

        /// <summary>
        ///     Update pattern figure
        /// </summary>
        private void DrawPatternFigure()
        {
            var tex = _easel.PatternFigure.Texture;
            var rect = new Rect(0, 0, _easel.PatternFigure.Texture.width, _easel.PatternFigure.Texture.height);
            var sprite = Sprite.Create(tex, rect, Vector2.zero);

            PatternImage.overrideSprite = sprite;
        }

        #endregion

        /// <summary>
        ///     Init Configuration Values and filling round list
        /// </summary>
        private void InitialValues()
        {
            // override default values
            if (TotalTime != default(int)) ConfigurationValues.StartTime = TotalTime;
            if (SimilarityThreshold != default(float)) ConfigurationValues.SimilarityThreshold = SimilarityThreshold;
            ScoreMainMenu.Player = _player;

            // Init round list
            IRound round;
            for (int i = 0; i < 100; i++)
            {
                if (i < 10) round = new Round(i, 4, Complexity.VeryLow);
                else if (i < 20) round = new Round(i, 3, Complexity.Low);
                else if (i < 30) round = new Round(i, 1.5f, Complexity.Normal);
                else if (i < 40) round = new Round(i, 1, Complexity.Hard);
                else round = new Round(i, 0.3f, Complexity.VeryHard);

                _roundList.Add(round);
            }
        }
    }
}
