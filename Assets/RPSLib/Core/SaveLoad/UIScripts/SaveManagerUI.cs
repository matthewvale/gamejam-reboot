/* Author:		Matthew Vale
 * Role:		Lead Game Developer
 * Company:		Red Phoenix Studios
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPSCore {

    public class SaveManagerUI : ManagedUI {

        #region VARIABLES - USE SUB-REGIONS 
        #region INSTANCING
        public static SaveManagerUI Instance;
        #endregion
        #region UI
        [Header("Save UI")]
        public Canvas saveGamesCanvas;
        public TMP_InputField saveFileNameInput;
        public Button saveButton;
        [Header("Confirmations")]
        public Canvas confirmOverwriteCanvas;
        public Canvas confirmDeleteCanvas;
        #endregion
        #region SAVE GAME UI ITEM PREFABS
        [Header("Save game items")]
        public GameObject saveGamePrefab;
        public RectTransform saveGameListParent;
        private List<GameObject> saveGameList = new List<GameObject>();
        #endregion
        #region SELECTED GAME SAVE
        private string selectedSaveGameFile;
        #endregion
        #region AUTO-SAVE FEATURE
        private readonly WaitForSecondsRealtime waitForRealSeconds = new(300f); // 600 = 10 mins
        #endregion
        #region Saveable Scenes
        [Header("Saveable Scenes")]
        public string[] saveableScenes; // Array of scenes we can save in. We don't want to save in the Main menu for example.
        #endregion
        #endregion


        #region SETUP
        protected void Awake() {
            RegisterManagedUI(saveGamesCanvas, false);
            RegisterManagedUI(confirmOverwriteCanvas, false);
            RegisterManagedUI(confirmDeleteCanvas, false);
            Instance = this;
            Init();
        }

        private void Init() {
            PopulateSaveGameList();
        }

        protected void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            StopAllCoroutines();
            if (saveableScenes.Contains(scene.name)) {
                StartCoroutine(nameof(AutosaveRun));
            }
        }
        #endregion

        #region INPUT
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape) && saveGamesCanvas.enabled) {
                CloseSaveGamesCanvas();
            }
        }
        #endregion

        #region UI ELEMENTS
        public void OpenSaveGamesCanvas() {
            PopulateSaveGameList();
            ShowCanvas(saveGamesCanvas, DisplayMethod.HideAllAndShowMe, true);
        }

        public void CloseSaveGamesCanvas() {
            HideCanvas(saveGamesCanvas);
        }

        public void CreateSaveGameItem(string saveGameID, DateTime saveGameDateTime) {
            GameObject saveGameItem = Instantiate(saveGamePrefab, saveGameListParent);
            SaveFileItem sfi = saveGameItem.GetComponent<SaveFileItem>();
            sfi.PassInFileNameAndDateTime(saveGameID, saveGameDateTime);
            saveGameItem.GetComponent<Button>().onClick.AddListener(delegate { SelectSaveGame(saveGameID); });
            saveGameList.Add(saveGameItem);
        }

        private void SelectSaveGame(string ID) {
            selectedSaveGameFile = ID;
            saveFileNameInput.text = selectedSaveGameFile;
            SelectedSaveGameFile.selectedSaveGameFileName = selectedSaveGameFile;
        }

        public void ToggleSaveButton(bool state) {
            saveButton.enabled = state;
        }
        #endregion

        #region SAVE GAME LIST
        private void PopulateSaveGameList() {
            ClearSaveGameList();

            // Read local save file folder
            if (!Directory.Exists(Application.persistentDataPath + "/saves/")) {
                Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
                RPSLib.Debug.Log("Creating /save/ directory.");
            }

            // Get and sort the save files
            string[] files = Directory.GetFiles(Application.persistentDataPath + "/saves/");
            string[] sortedFiles = files.OrderByDescending(f => File.GetLastWriteTime(f)).ToArray();

            for (int i = 0; i < sortedFiles.Length; i++) {
                if (Path.GetExtension(sortedFiles[i]) == ".json") {
                    CreateSaveGameItem(Path.GetFileNameWithoutExtension(sortedFiles[i]), File.GetLastWriteTime(sortedFiles[i]));
                }
            }
        }

        private void ClearSaveGameList() {
            for (int i = 0; i < saveGameList.Count; i++) {
                Destroy(saveGameList[i]);
            }
            saveGameList.Clear();
        }
        #endregion

        #region AUTO-SAVE FEATURE
        private IEnumerator AutosaveRun() {
            yield return waitForRealSeconds;
            AutoSaveGame();
            StartCoroutine(nameof(AutosaveRun));
        }
        #endregion

        #region LOAD LATEST GAME (CONTINUE)
        public void LoadLastSavedGame() {
            SelectSaveGame(saveGameList[0].GetComponent<SaveFileItem>().fileName.text);
            ClickLoadGame();
        }

        public string GetLastSavedGame() {
            if (saveGameList.Count == 0)
                return "";

            return saveGameList[0].GetComponent<SaveFileItem>().fileName.text;
        }
        #endregion

        #region PUBLIC METHODS
        public void ClearSelectedSaveFile() {
            selectedSaveGameFile = "";
        }

        public void ClickSaveGame() {
            if (saveFileNameInput.text == "" || saveFileNameInput.text == null)
                return;

            if (SaveManager.Instance.SaveFileExists(saveFileNameInput.text)) {
                ShowCanvas(confirmOverwriteCanvas, DisplayMethod.ShowMeAboveExisting, true);
                return;
            }

            SaveManager.Instance.SaveGame(saveFileNameInput.text);
            PopulateSaveGameList();
        }

        public void ConfirmSaveOverwriteGame() {
            SaveManager.Instance.SaveGame(saveFileNameInput.text);
            PopulateSaveGameList();
            HideCanvas(confirmOverwriteCanvas);
        }


        public void CancelSaveOverwrite() {
            HideCanvas(confirmOverwriteCanvas);
        }

        private void AutoSaveGame() {
            //if (GameSaveData.Instance.PlayerName != null) {
            //    SaveManager.Instance.SaveGame("Autosave - " + GameSaveData.Instance.PlayerName);
            //}
        }

        public void ClickLoadGame() {
            if (selectedSaveGameFile == "" || selectedSaveGameFile == null) {
                return;
            }
            SaveManager.Instance.LoadGame(selectedSaveGameFile);
            CloseSaveGamesCanvas();
            selectedSaveGameFile = null;
        }

        public void ClickDeleteGame() {
            if (selectedSaveGameFile == "" || selectedSaveGameFile == null)
                return;

            ShowCanvas(confirmDeleteCanvas, DisplayMethod.ShowMeAboveExisting, true);
        }

        public void ConfirmDeleteGame() {
            SaveManager.Instance.DeleteSaveGame(selectedSaveGameFile);
            PopulateSaveGameList();
            HideCanvas(confirmDeleteCanvas);
        }

        public void CancelDeleteGame() {
            HideCanvas(confirmDeleteCanvas);
        }

        public void PauseCameraControl() {
            //CameraControllerV2.Instance.RemoveCameraControl();
        }

        public void ResumeCameraControl() {
            //CameraControllerV2.Instance.GiveCameraControl();
        }
        #endregion

    }

}