using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using NXOpen;

namespace PartList
{

    #region File: ListModel.cs
    public class Part
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string Quantity { get; set; }  // Ví dụ "6EA"
        public string Size { get; set; }
        public string Material { get; set; }
        public string Remarks { get; set; }
        public string ImagePath { get; set; }

        public Part(string partNo, string partName, string quantity, string size, string material, string remarks, string imagePath)
        {
            PartNo = partNo;
            PartName = partName;
            Quantity = quantity;
            Size = size;
            Material = (material.Trim() == "0" ? "" : material);
            Remarks = remarks;
            ImagePath = imagePath;
        }
    }

    public interface IPartListService
    {
        List<Part> LoadParts();
        void UpdatePart(Part part);
        void RemovePart(string partNo);
    }

    public class PartListService : IPartListService
    {
        private readonly string xmlFilePath;

        public PartListService()
        {
            Session session = Session.GetSession();
            string partFullPath = session.Parts.Work.FullPath;
            string folderPath = Path.GetDirectoryName(partFullPath);
            xmlFilePath = Path.Combine(folderPath, "xmlList.xml");
        }

        public List<Part> LoadParts()
        {
            List<Part> parts = new List<Part>();
            if (File.Exists(xmlFilePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFilePath);
                XmlNodeList partNodes = doc.GetElementsByTagName("Part");
                foreach (XmlNode node in partNodes)
                {
                    string partNo = (node["PartNo"] != null ? node["PartNo"].InnerText : "");
                    string partName = (node["PartName"] != null ? node["PartName"].InnerText : "");
                    string quantity = (node["Quantity"] != null ? node["Quantity"].InnerText : "");
                    string size = (node["Size"] != null ? node["Size"].InnerText : "");
                    string material = (node["Material"] != null ? node["Material"].InnerText : "");
                    string remarks = (node["Remark"] != null ? node["Remark"].InnerText : (node["Remarks"] != null ? node["Remarks"].InnerText : ""));
                    string imagePath = (node["Image"] != null ? node["Image"].InnerText : "");
                    Part part = new Part(partNo, partName, quantity, size, material, remarks, imagePath);
                    parts.Add(part);
                }
            }
            return parts;
        }

        public void UpdatePart(Part part)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                if (File.Exists(xmlFilePath))
                {
                    doc.Load(xmlFilePath);
                    XmlNode existingPart = null;
                    foreach (XmlNode node in doc.GetElementsByTagName("Part"))
                    {
                        if (node["PartNo"] != null && node["PartNo"].InnerText == part.PartNo)
                        {
                            existingPart = node;
                            break;
                        }
                    }
                    if (existingPart != null)
                    {
                        existingPart["PartName"].InnerText = part.PartName;
                        existingPart["Quantity"].InnerText = part.Quantity;
                        existingPart["Size"].InnerText = part.Size;
                        existingPart["Material"].InnerText = part.Material;
                        existingPart["Remark"].InnerText = part.Remarks;
                        existingPart["Image"].InnerText = part.ImagePath;
                    }
                    else
                    {
                        XmlElement newPart = doc.CreateElement("Part");
                        newPart.AppendChild(CreateElement(doc, "PartNo", part.PartNo));
                        newPart.AppendChild(CreateElement(doc, "PartName", part.PartName));
                        newPart.AppendChild(CreateElement(doc, "Quantity", part.Quantity));
                        newPart.AppendChild(CreateElement(doc, "Size", part.Size));
                        newPart.AppendChild(CreateElement(doc, "Material", part.Material));
                        newPart.AppendChild(CreateElement(doc, "Type", ""));
                        newPart.AppendChild(CreateElement(doc, "Remark", part.Remarks));
                        newPart.AppendChild(CreateElement(doc, "Image", part.ImagePath));

                        if (doc.DocumentElement != null)
                        {
                            doc.DocumentElement.AppendChild(newPart);
                        }
                        else
                        {
                            XmlElement root = doc.CreateElement("Parts");
                            root.AppendChild(newPart);
                            doc.AppendChild(root);
                        }
                    }
                }
                else
                {
                    XmlDocument newDoc = new XmlDocument();
                    XmlElement root = newDoc.CreateElement("Parts");
                    XmlElement newPart = newDoc.CreateElement("Part");

                    newPart.AppendChild(CreateElement(newDoc, "PartNo", part.PartNo));
                    newPart.AppendChild(CreateElement(newDoc, "PartName", part.PartName));
                    newPart.AppendChild(CreateElement(newDoc, "Quantity", part.Quantity));
                    newPart.AppendChild(CreateElement(newDoc, "Size", part.Size));
                    newPart.AppendChild(CreateElement(newDoc, "Material", part.Material));
                    newPart.AppendChild(CreateElement(newDoc, "Type", ""));
                    newPart.AppendChild(CreateElement(newDoc, "Remark", part.Remarks));
                    newPart.AppendChild(CreateElement(newDoc, "Image", part.ImagePath));

                    root.AppendChild(newPart);
                    newDoc.AppendChild(root);
                    doc = newDoc;
                }
                doc.Save(xmlFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("XML Update Fail: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RemovePart(string partNo)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                if (File.Exists(xmlFilePath))
                {
                    doc.Load(xmlFilePath);
                    XmlNode nodeToRemove = null;
                    foreach (XmlNode node in doc.GetElementsByTagName("Part"))
                    {
                        if (node["PartNo"] != null && node["PartNo"].InnerText == partNo)
                        {
                            nodeToRemove = node;
                            break;
                        }
                    }
                    if (nodeToRemove != null)
                    {
                        nodeToRemove.ParentNode.RemoveChild(nodeToRemove);
                        doc.Save(xmlFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Remove Part Fail: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private XmlElement CreateElement(XmlDocument doc, string name, string value)
        {
            XmlElement element = doc.CreateElement(name);
            element.InnerText = value;
            return element;
        }
    }
    #endregion


}

