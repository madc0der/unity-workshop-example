using DefaultNamespace.common;
using stat;
using TMPro;
using UnityEngine;

namespace menu
{
    public class BestScorePanelController : AbstractPanelController
    {
        public int limit;
        public GameObject rowTemplate;
        public GameObject rowsContainer;

        private static AbstractPanelController instance;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        public static AbstractPanelController GetInstance()
        {
            return instance;
        }

        private void OnEnable()
        {
            var records = UserStatController.GetBestScore(limit);
            HierarchyHelper.ClearChildren(rowsContainer, rowTemplate);
            
            foreach (var record in records)
            {
                var clone = Instantiate(rowTemplate, rowsContainer.transform);
                clone.SetActive(true);
                var rowTransform = clone.transform;
                rowTransform.GetChild(0).GetComponent<TMP_Text>().text = record.name;
                rowTransform.GetChild(1).GetComponent<TMP_Text>().text = $"{record.bestScore}";
            }
        }
    }
}