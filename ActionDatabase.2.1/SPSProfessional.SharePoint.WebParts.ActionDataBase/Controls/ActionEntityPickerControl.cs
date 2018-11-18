using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.WebParts.ActionDataBase.Relations;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase.Controls
{
    public class ActionEntityPickerControl : EntityEditorWithPicker
    {
        private ActionEntity _extendedData;

        #region Properties

        /// <summary>
        /// Gets or sets the extended data.
        /// Essential to get data from the popup
        /// </summary>
        /// <value>The extended data.</value>
        /// <exception cref="ArgumentNullException"><c>value</c> is null.</exception>
        public ActionEntity ExtendedData
        {
            get
            {
                if (_extendedData == null)
                {
                    try
                    {
                        byte[] buffer = Convert.FromBase64String(CustomProperty);
                        BinaryFormatter formatter = new BinaryFormatter();
                        MemoryStream serializationStream = new MemoryStream(buffer);
                        _extendedData = (ActionEntity) formatter.Deserialize(serializationStream);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                return _extendedData;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream serializationStream = new MemoryStream();
                formatter.Serialize(serializationStream, value);
                serializationStream.Close();
                CustomProperty = Convert.ToBase64String(serializationStream.GetBuffer());
            }
        }

        #endregion

        public ActionEntityPickerControl()
        {
            Debug.WriteLine("%%%%%% ActionEntityPickerControl");
            EnableBrowse = true;
            EntitySeparator = '\0';
            AllowTypeIn = true;
            ValidatorEnabled = true;
            Title = "Action Picker";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            PickerDialogType = typeof(ActionPickerDialog);
            BrowseButtonImageName = "/_layouts/images/gortl.gif";
        }

        /// <summary>
        /// Validates the entity.
        /// </summary>
        /// <param name="needsValidation">The needs validation.</param>
        /// <returns>The entity validated</returns>
        public override PickerEntity ValidateEntity(PickerEntity needsValidation)
        {
            Debug.WriteLine("%%%%%% ValidateEntity " + needsValidation.Key);

            if (ExtendedData == null)
            {
                needsValidation.IsResolved = false;
            }
            else
            {
                ActionEntityDataBase actionEntityDataBase = new ActionEntityDataBase(ExtendedData);
                actionEntityDataBase.ResolvePickerEntity(needsValidation);
            }

            return needsValidation;
        }

        /// <summary>
        /// Resolves the error by search.
        /// </summary>
        /// <param name="unresolvedText">The unresolved text.</param>
        /// <returns>An array with entities that match</returns>
        protected override PickerEntity[] ResolveErrorBySearch(string unresolvedText)
        {
            Debug.WriteLine("%%%%%% ResolveErrorBySearch " + unresolvedText);

            if (ExtendedData != null)
            {
                ActionEntityDataBase actionEntitydataBase = new ActionEntityDataBase(ExtendedData);
                return actionEntitydataBase.MatchesEntities(unresolvedText).ToArray();
            }

            return new PickerEntity[] {};
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public override void Validate()
        {
            Debug.WriteLine("%%%%%%% Validate " + IsValid);
            base.Validate();
            Debug.WriteLine("%%%%%%% End Validate " + IsValid);
        }
    }

}