using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay{
    [System.Serializable]
    public class PlayerActions
    {
        [SerializeField] Camera Cam;
        [SerializeField] LineRenderer TrajectoryLine;
        [SerializeField] BoxCollider2D LeftBarrier, RightBarrier;
        [SerializeField] Vector2 RootPoint;
        [SerializeField, Range (1, 10)] int TrajectoryCornersSmooth;
        bool GameplayActive, DirectionValid;
        float BubbleSize;
        Vector2 RayOrigin, RayDir, MousePos;
        Bubble ActualBubble;
        int BubbleSuccessCollisions;
        Vector3[] BubbleWaypoints, TrajectoryPoints;
        RaycastHit2D CollisionData;
        BubblesQueue Queue;
        GameField GameField;
        [SerializeField] EventSystem Events;
        [SerializeField] UnityEngine.UI.GraphicRaycaster[] UserCanvases;
        List<RaycastResult> CanvasResults;
        PointerEventData m_PointerEventData;
        
        public void Init(float BubbleSize, BubblesQueue Queue, GameField Field)
        {
            Cam = Camera.main;
            BubbleWaypoints = new Vector3[20];
            TrajectoryPoints = new Vector3[50];
            this.BubbleSize = BubbleSize; 
            TrajectoryLine.startWidth = BubbleSize;
            this.Queue = Queue;
            GameField = Field;
        }
        
        public void StopGameplay(Pool HidingPool)
        {
            GameplayActive = false;
            TrajectoryLine.gameObject.SetActive(false);
            if (ActualBubble == null) return;
            HidingPool.HideBubbleToPool(ActualBubble);
            ActualBubble = null;
        }
        
        public void StartGameplay()
        {
            GameplayActive = true;
            TrajectoryLine.gameObject.SetActive(true);
            RefreshActualBubble();
        }
        
        void RefreshActualBubble()
        {
            if (ActualBubble != null) return;
            ActualBubble = Queue.GiveMeBubble();
            ActualBubble.MyTransform.position = RootPoint;
            ActualBubble.DeactivateCollisions();
        }
        
        public void ApplyPointerPos(Vector2 NewPos)
        {
            if (MousePos == NewPos) return;
            MousePos = NewPos;
            ProcessRay();
        }
        
        void ProcessRay()
        {
            if (!GameplayActive) return;
            SetupStarts();
            CheckValid();
            if (!DirectionValid) return;
            int TrajectoryPointsCount = 1;
            float CircleRound = BubbleSize*0.5f;
            Vector2 ForwardRound, UpRound, RoundCenter, HitPos;
            for (BubbleSuccessCollisions = 1; BubbleSuccessCollisions<20; BubbleSuccessCollisions++)
            {
                CollisionData = Physics2D.Raycast(RayOrigin, RayDir, 20);
                if (CollisionData.collider == null) 
                {
                    HitPos = RayOrigin + RayDir * 10;
                    if (TrajectoryPointsCount < TrajectoryPoints.Length)
                    {
                        TrajectoryPoints[TrajectoryPointsCount] = HitPos;
                        TrajectoryPointsCount++;
                    }
                    BubbleWaypoints[BubbleSuccessCollisions] = HitPos;
                    BubbleSuccessCollisions++;
                    break;
                }
                else if (CollisionData.collider == LeftBarrier || CollisionData.collider == RightBarrier)
                {
                    CollisionData.point -= RayDir * Mathf.Sin(Vector2.Angle(-CollisionData.normal, RayDir)* Mathf.Deg2Rad) * BubbleSize;
                    var NewDir = Vector2.Reflect(RayDir, CollisionData.normal).normalized;
                    if (TrajectoryPointsCount < TrajectoryPoints.Length)
                    {
                        RoundCenter = CollisionData.point + CollisionData.normal * CircleRound;
                        var Angle = Vector2.Angle(RayDir, NewDir);
                        float CurrentAngleInRad = 0;
                        ForwardRound = -CollisionData.normal;
                        UpRound = new Vector2(ForwardRound.y, Mathf.Abs(ForwardRound.x));
                        for (int k = 0; k < TrajectoryCornersSmooth + 2; k++)
                        {
                            CurrentAngleInRad = Angle * (-0.5f + (k/(float)(TrajectoryCornersSmooth+2))) * Mathf.Deg2Rad;
                            TrajectoryPoints[TrajectoryPointsCount] = RoundCenter + (ForwardRound * Mathf.Cos(CurrentAngleInRad) + UpRound * Mathf.Sin(CurrentAngleInRad)) * CircleRound; 
                            TrajectoryPointsCount++;
                            if (TrajectoryPointsCount >= TrajectoryPoints.Length) break;
                        }
                    }
                    RayOrigin = CollisionData.point;
                    RayDir = NewDir;
                    RayOrigin += RayDir * 0.05f;
                    BubbleWaypoints[BubbleSuccessCollisions] = RayOrigin;
                }
                else 
                {
                    if (TrajectoryPointsCount < TrajectoryPoints.Length)
                    {
                        TrajectoryPoints[TrajectoryPointsCount] = CollisionData.point;
                        TrajectoryPointsCount++;    
                    }
                    BubbleWaypoints[BubbleSuccessCollisions] = CollisionData.point - RayDir * BubbleSize*0.5f;
                    BubbleSuccessCollisions++;
                    break;
                }
                
            }
            TrajectoryLine.positionCount = TrajectoryPointsCount;
            TrajectoryLine.SetPositions(TrajectoryPoints);
            
            
            void CheckValid()
            {
                var StartAngle = Vector2.Angle(Vector2.up, RayDir) * 2;
                DirectionValid = StartAngle < 140;
            }
            
            void SetupStarts()
            {
                RayOrigin = RootPoint;
                RayDir = (Vector2)Cam.ScreenToWorldPoint(MousePos) - RootPoint;
                
                RayDir = RayDir.normalized;
                BubbleWaypoints[0] = RayOrigin;
                TrajectoryPoints[0] = RayOrigin;
            }
        }
        
        public void ShootBubble()
        {
            if (ActualBubble == null) return;
            if (IsClickedToCanvases()) return;
            ProcessRay();
            if (!DirectionValid) return;
            Vector3[] Waypoints = new Vector3[BubbleSuccessCollisions+1];
            for (int i=0; i< Waypoints.Length; i++)
            {
                Waypoints[i] = BubbleWaypoints[i];
            }
            ActualBubble.ActivateCollisions();
            AnimateBubble(ActualBubble, Waypoints);
            ActualBubble = null;
        }
        
        bool IsClickedToCanvases()
        {
            foreach(var Canvas in UserCanvases)
            {
                if (!Canvas.gameObject.activeInHierarchy) continue;
                if (!isClickedToCanvas(Canvas)) continue;
                return true;
            }
            return false;
        }
        
        bool isClickedToCanvas(UnityEngine.UI.GraphicRaycaster Target)
        {
            m_PointerEventData = new PointerEventData(Events);
            m_PointerEventData.position = MousePos;
            if (CanvasResults == null) CanvasResults = new List<RaycastResult>();
            CanvasResults.Clear();
            Target.Raycast(m_PointerEventData, CanvasResults);
            return CanvasResults.Count > 0;
        }
        
        async void AnimateBubble(Bubble TargetBubble, Vector3[] Waypoints)
        {
            const float Speed = 0.3f;
            TargetBubble.MyTransform.position = Waypoints[0];
            Vector3 Direction = (Waypoints[1] - Waypoints[0]).normalized * Speed; 
            Vector3 EndPoint = Waypoints[1];
            int CurrentPoint = 0;
            while(CurrentPoint < Waypoints.Length-1)
            {
                if (!Application.isPlaying) return;
                if ((EndPoint - TargetBubble.MyTransform.position).magnitude < Speed)
                {
                    TargetBubble.MyTransform.position = EndPoint;
                    Direction = (Waypoints[CurrentPoint+1] - Waypoints[CurrentPoint]).normalized * Speed; 
                    EndPoint = Waypoints[CurrentPoint+1];
                    CurrentPoint++;
                }
                else 
                {
                    TargetBubble.MyTransform.position += Direction;
                }
                await System.Threading.Tasks.Task.Delay(16);
            }
            TargetBubble.ActivateCollisions();
            GameField.PlaceUserBubble(TargetBubble);
            if (!GameplayActive) return;
            RefreshActualBubble();
            ProcessRay();
        }
        
        public void SwitchBubbleToSecondary()
        {
            ActualBubble = Queue.SwitchBubbles(ActualBubble);
            ActualBubble.MyTransform.position = RootPoint;
            ActualBubble.DeactivateCollisions();
        }
    }
}