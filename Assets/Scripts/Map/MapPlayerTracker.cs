using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map
{
    public class MapPlayerTracker : MonoBehaviour
    {
        public bool lockAfterSelecting = false;
        public float enterNodeDelay = 1f;
        public MapManager mapManager;
        public MapView view;
        

        public static MapPlayerTracker Instance;

        public bool Locked { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        public void SelectNode(MapNode mapNode)
        {
            if (Locked) return;

            // Debug.Log("Selected node: " + mapNode.Node.point);

            if (mapManager.CurrentMap.path.Count == 0)
            {
                // player has not selected the node yet, he can select any of the nodes with y = 0
                if (mapNode.Node.point.y == 0)
                    SendPlayerToNode(mapNode);
                else
                    PlayWarningThatNodeCannotBeAccessed();
            }
            else
            {
                var currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
                var currentNode = mapManager.CurrentMap.GetNode(currentPoint);

                if (currentNode != null && currentNode.outgoing.Any(point => point.Equals(mapNode.Node.point)))
                    SendPlayerToNode(mapNode);
                else
                    PlayWarningThatNodeCannotBeAccessed();
            }
        }

        private void SendPlayerToNode(MapNode mapNode)
        {
            mapNode.ShowSwirlAnimation();
            Locked = lockAfterSelecting;
            mapManager.CurrentMap.path.Add(mapNode.Node.point);
            mapManager.SaveMap();
            view.SetAttainableNodes();
            view.SetLineColors();
            mapNode.ShowSwirlAnimation();


            DOTween.Sequence().AppendInterval(enterNodeDelay).OnComplete(() => EnterNode(mapNode));


        }


        public static void OpenScene(String sceneName){
            SceneManager.LoadScene(sceneName);
        }

        public void returnToTree(){
            SceneManager.LoadScene("SampleSceneMap");
            Time.timeScale = 1.0f;
        }

        private static void EnterNode(MapNode mapNode)
        {
            // we have access to blueprint name here as well
            UnityEngine.Debug.Log("Entering node: " + mapNode.Node.blueprintName + " of type: " + mapNode.Node.nodeType);
            // load appropriate scene with context based on nodeType:
            // or show appropriate GUI over the map: 
            // if you choose to show GUI in some of these cases, do not forget to set "Locked" in MapPlayerTracker back to false
            switch (mapNode.Node.nodeType)
            {
                case NodeType.MinorEnemy:
                    PlayerInventory.Instance.battlesFought++;
                    OpenScene("sampleEnemyFormations");
                    break;
                case NodeType.EliteEnemy:
                    break;
                case NodeType.RestSite:
                    break;
                case NodeType.Treasure:
                    break;
                case NodeType.Store:
                    break;
                case NodeType.Boss:
                    PlayerInventory.Instance.battlesFought++;
                    PlayerInventory.Instance.finalBoss = true;
                    OpenScene("sampleEnemyFormations");
                    break;
                case NodeType.Mystery:
                    OpenScene("eventScene");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            int percentage = PlayerInventory.Instance.battlesFought/11;
            GameMetrics.Instance.mapPercentage=percentage;
        }

        private void PlayWarningThatNodeCannotBeAccessed()
        {
            UnityEngine.Debug.Log("Selected node cannot be accessed");
        }
    }
}