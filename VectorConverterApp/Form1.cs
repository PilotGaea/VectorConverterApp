using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using PilotGaea.Serialize;
using PilotGaea.TMPEngine;
using PilotGaea.Geometry;

namespace VectorConverterApp
{
    public partial class Form1 : Form
    {
        CVectorMaker m_Maker = null;
        Stopwatch m_Stopwatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();

            //加入功能列表
            List<string> featureNames = new List<string>();
            featureNames.Add("基本");
            comboBox_Features.Items.AddRange(featureNames.ToArray());
            comboBox_Features.SelectedIndex = 0;
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            EnableUI(false);

            //     生成一向量圖層(來源為shp)
            System.Environment.CurrentDirectory = @"C:\Program Files\PilotGaea\TileMap";//為了順利存取安裝目錄下的相關DLL
            m_Maker = new CVectorMaker();
            //設定必要參數
            //     目標DB路徑
            string DBFileName = string.Format(@"{0}\..\output\vector_maker.DB", Application.StartupPath);
            //     地形DB路徑
            string TerrainDBName = string.Format(@"{0}\..\data\terrain_maker\terrain.DB", Application.StartupPath);
            //     地形名稱
            string TerrainLayerName = "terrain";
            //     圖層名稱
            string LayerName = "test";
            //     來源shp檔路徑
            string ShpFileName = string.Format(@"{0}\..\data\vector_maker\gis_osm_places_free_1.shp", Application.StartupPath);
            //     顯示文字欄位編號
            int TextFieldIndex = 0;
            //     顯示圖標欄位編號，可利用SymbolFieldValue及ImagePath參數做進一步設定
            int SymbolFieldIndex = 1;
            //     來源EPSG
            int SrcEPSG = 4326;
            //     是否使用叢集
            bool UseCluster = true;
            //     是否使用簡約化
            bool UseSimplePoint = false;
            //     群集點的最大合併距離
            double MaxClusterDistance = 200;
            //     是否使用欄位高度值(不使用則永遠取地形高)
            bool UseHeight = false;
            //     高度欄位編號，UseHeight若為false則無意義
            int HeightIndex = 0;
            //     高度欄位值是否為絕對高，UseHeight若為false則無意義
            bool IsAbsoluteHeight = false;
            //     與ImagePath參數相對應，當設定的顯示圖標欄位中的值存在於此參數陣列中時，則將該點圖標設為編號對應於ImagePath陣列中的圖形檔，另外，"d"
            string[] SymbolFieldValue = null;
            //     與SymbolFieldValue參數相對應，記錄圖形檔完整路徑，根據SymbolFieldValue參數將圖素的圖標設為對應的圖形檔
            string[] ImagePath = null;
            //     顯示文字字體設定，陣列長度固定為9，參數分別代表{字型名稱、字型顏色R、G、B、字型邊界顏色R、G、B、字型大小，字型邊界大小}，預設值為{微軟正黑體,255,255,255,0,0,0,16,3}
            string[] TextSet = null;


            //監聽轉檔事件
            m_Maker.CreateLayerCompleted += M_Maker_CreateLayerCompleted;
            m_Maker.ProgressMessageChanged += M_Maker_ProgressMessageChanged;
            m_Maker.ProgressPercentChanged += M_Maker_ProgressPercentChanged;

            //設定進階參數
            switch (comboBox_Features.SelectedIndex)
            {
                case 0://"基本"
                    break;
            }

            m_Stopwatch.Restart();
            //開始非同步轉檔
            bool ret = m_Maker.Create(DBFileName, TerrainDBName, TerrainLayerName, ShpFileName, TextFieldIndex, SymbolFieldIndex, LayerName, SrcEPSG, UseCluster, UseSimplePoint, MaxClusterDistance, UseHeight, HeightIndex, IsAbsoluteHeight,
                SymbolFieldValue, ImagePath, TextSet);
            string message = string.Format("Create檢查{0}", (ret ? "通過" : "失敗"));
            listBox_Main.Items.Add(message);
        }

        private void M_Maker_CreateLayerCompleted(string LayerName, bool Success, string ErrorMessage)
        {
            m_Stopwatch.Stop();
            string message = string.Format("CreateCompleted{0}", (Success ? "成功" : "失敗"));
            listBox_Main.Items.Add(message);
            message = string.Format("耗時{0}分。", m_Stopwatch.Elapsed.TotalMinutes.ToString("0.00"));
            listBox_Main.Items.Add(message);
        }

        private void M_Maker_ProgressPercentChanged(double Percent)
        {
            progressBar_Main.Value = Convert.ToInt32(Percent);
        }

        private void M_Maker_ProgressMessageChanged(string Message)
        {
            listBox_Main.Items.Add(Message);
        }

        private void EnableUI(bool enable)
        {
            button_Start.Enabled = enable;
            comboBox_Features.Enabled = enable;
        }
    }
}