using System.Collections.Generic;
using Domain;
using UnityEngine;

namespace App
{
    public class DrawingController : IDrawingController
    {
        private bool _isMousePressed = false;
        private List<Vector3> _pointsList = new List<Vector3>();
        private Vector3 _mousePos;

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="line">LineRenderer object used for drawing</param>
        /// <param name="trail">ParticleSystem Effect when drawing</param>
        /// <param name="board">Main board</param>
        public DrawingController(LineRenderer line, ParticleSystem trail, IBoard board)
        {
            Utils.ThrowIfNull(line, "LineRenderer not set to instance of DrawingController");
            Utils.ThrowIfNull(trail, "TrailEffect not set to instance of DrawingController");
            Utils.ThrowIfNull(board, "Board not set to instance of DrawingController");

            Line = line;
            TrailEffect = trail;
            Board = board;
        }

        public LineRenderer Line { get; private set; }
        public ParticleSystem TrailEffect { get; private set; }
        public IBoard Board { get; private set; }
        public bool DrawingIsOver { get; private set; }

        public void Update()
        {
            // If mouse button down, remove old line
            if (Input.GetMouseButtonDown(0))
            {
                Clear();
                _isMousePressed = true;
                Line.gameObject.SetActive(true);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                DefineTextureBorder();
                _isMousePressed = false;
                DrawingIsOver = true;
            }

            // Drawing line when mouse is moving(presses)
            if (_isMousePressed)
            {
                _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _mousePos.z = 0;

                if (!_pointsList.Contains(_mousePos))
                {
                    _pointsList.Add(_mousePos);
                    Line.positionCount = _pointsList.Count;
                    Line.SetPosition(_pointsList.Count - 1, _pointsList[_pointsList.Count - 1]);
                    TrailEffect.gameObject.transform.position = _mousePos;
                }
            }
        }

        /// <summary>
        ///     Clears LineRenderer
        /// </summary>
        public void Clear()
        {
            DrawingIsOver = false;
            Line.positionCount = 0;
            _pointsList.RemoveRange(0, _pointsList.Count);
            Line.gameObject.SetActive(false);
        }

        /// <summary>
        ///     Identifies the key points of the custom figure
        /// </summary>
        private void DefineTextureBorder()
        {
            if(_pointsList.Count == 0) return;

            var minX = float.MaxValue;
            var maxX = 0f;
            var minY = float.MaxValue;
            var naxY = 0f;

            foreach (var point in _pointsList)
            {
                if (point.x < minX) minX = point.x;
                if (point.x > maxX) maxX = point.x;

                if (point.y < minY) minY = point.y;
                if (point.y > naxY) naxY = point.y;
            }

            Board.MinPosition = Camera.main.WorldToScreenPoint(new Vector3(minX, minY, 0));
            Board.MaxPosition = Camera.main.WorldToScreenPoint(new Vector3(maxX, naxY, 0));
        }
    }
}