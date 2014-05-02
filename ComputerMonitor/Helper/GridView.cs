using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace ComputerMonitor.Helper
{
    class GridView
    {
        

       

      

        private static SourceGrid.Cells.Views.Header headerView = null;
        public static SourceGrid.Cells.Views.Header HeaderView
        {
            get 
            {
                if (headerView == null)
                {
                    headerView = new SourceGrid.Cells.Views.Header();
                    headerView.Font = new Font("ËÎÌו",13);
                    headerView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                }
                return headerView;
            }
        }

        private static SourceGrid.Cells.Views.Cell selectionView;
        public static SourceGrid.Cells.Views.Cell SelectionView
        {
            get
            {
                if (selectionView == null)
                {
                    //Border
                    //DevAge.Drawing.BorderLine border = new DevAge.Drawing.BorderLine(Color.Black);
                    //DevAge.Drawing.RectangleBorder cellBorder = new DevAge.Drawing.RectangleBorder(border, border);
                    //Selection View
                    selectionView = new SourceGrid.Cells.Views.Cell();
                    //selectionView.Background = new DevAge.Drawing.VisualElements.BackgroundSolid(Color.FromArgb(220, 255, 255));
                    selectionView.ImageAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                    //selectionView.Border = cellBorder;
                }
                return selectionView;
            }
        }

        private static SourceGrid.Cells.Views.Cell normalView;
        public static SourceGrid.Cells.Views.Cell NormalView
        {
            get
            {
                if (normalView == null)
                {
                    
                    //Data Normal View
                    normalView = new SourceGrid.Cells.Views.Cell();
                    normalView.Font = new Font("ËÎÌו",15);
                    normalView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                    
                }
                return normalView;
            }
        }

        private static SourceGrid.Cells.Views.Cell dataNormalView;
        public static SourceGrid.Cells.Views.Cell DataNormalView
        {
            get
            {
                if (dataNormalView == null)
                {

                    //Data Normal View
                    dataNormalView = new SourceGrid.Cells.Views.Cell();
                    dataNormalView.Font = new Font("ËÎÌו", 15);
                    //dataNormalView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;

                }
                return dataNormalView;
            }
        }
        private static SourceGrid.Cells.Views.Cell dataFailView;
        public static SourceGrid.Cells.Views.Cell DataFailView
        {
            get
            {
                if (dataFailView == null)
                {
                   
                    //Data Fail View
                    dataFailView = new SourceGrid.Cells.Views.Cell();
                    dataFailView.ForeColor = Color.Red;
                    //dataFailView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                    
                }
                return dataFailView;
            }
        }

        private static SourceGrid.Cells.Views.Cell dataPassView;
        public static SourceGrid.Cells.Views.Cell DataPassView
        {
            get
            {
                if (dataPassView == null)
                {
                    
                    //Data Pass View
                    dataPassView = new SourceGrid.Cells.Views.Cell();
                    dataPassView.ForeColor = Color.Green;
                    dataPassView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
                }
                return dataPassView;
            }
        }

        
    }

   
}
