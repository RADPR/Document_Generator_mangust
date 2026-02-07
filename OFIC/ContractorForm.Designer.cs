namespace OFIC
{
    partial class ContractorForm
    {
        private System.ComponentModel.IContainer components = null;

        // Основные элементы формы
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        // Основная панель со всеми основными полями
        private System.Windows.Forms.Panel panelMainFields;

        // Динамические панели (появляются снизу в зависимости от типа)
        private System.Windows.Forms.Panel panelOOO;
        private System.Windows.Forms.Panel panelIP;
        private System.Windows.Forms.Panel panelFL;

        // Основные поля управления (общие для всех типов)
        private System.Windows.Forms.TextBox txtContractNumber;
        private System.Windows.Forms.ComboBox cmbContractorType;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.ComboBox cmbPropertyType;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.TextBox txtCadNumber;
        private System.Windows.Forms.ComboBox cmbSignalType;
        private System.Windows.Forms.TextBox txtMonthlyAmount;
        private System.Windows.Forms.ComboBox cmbBuildingType;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtPhone;

        // Метки для полей
        private System.Windows.Forms.Label lblContractNumber;
        private System.Windows.Forms.Label lblContractorType;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.Label lblPropertyType;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblCadNumber;
        private System.Windows.Forms.Label lblSignalType;
        private System.Windows.Forms.Label lblMonthlyAmount;
        private System.Windows.Forms.Label lblBuildingType;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblPhone;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelMainFields = new System.Windows.Forms.Panel();
            this.lblContractNumber = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblCadNumber = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblContractorType = new System.Windows.Forms.Label();
            this.lblMonthlyAmount = new System.Windows.Forms.Label();
            this.lblSignalType = new System.Windows.Forms.Label();
            this.lblPropertyType = new System.Windows.Forms.Label();
            this.lblBuildingType = new System.Windows.Forms.Label();
            this.txtContractNumber = new System.Windows.Forms.TextBox();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtCadNumber = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.cmbContractorType = new System.Windows.Forms.ComboBox();
            this.txtMonthlyAmount = new System.Windows.Forms.TextBox();
            this.cmbSignalType = new System.Windows.Forms.ComboBox();
            this.cmbPropertyType = new System.Windows.Forms.ComboBox();
            this.cmbBuildingType = new System.Windows.Forms.ComboBox();
            this.panelOOO = new System.Windows.Forms.Panel();
            this.panelIP = new System.Windows.Forms.Panel();
            this.panelFL = new System.Windows.Forms.Panel();
            this.panelHeader.SuspendLayout();
            this.panelMainFields.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(88)))), ((int)(((byte)(177)))));
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1200, 70);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1200, 70);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "НОВЫЙ КОНТРАГЕНТ";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(950, 450); // ИЗМЕНЕНО: уменьшен Y
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 40);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Visible = true; // ДОБАВЛЕНО: всегда видимые
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(1060, 450); // ИЗМЕНЕНО: уменьшен Y
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = true; // ДОБАВЛЕНО: всегда видимые
            // 
            // panelMainFields
            // 
            this.panelMainFields.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            this.panelMainFields.Controls.Add(this.lblContractNumber);
            this.panelMainFields.Controls.Add(this.lblFullName);
            this.panelMainFields.Controls.Add(this.lblAddress);
            this.panelMainFields.Controls.Add(this.lblCadNumber);
            this.panelMainFields.Controls.Add(this.lblEmail);
            this.panelMainFields.Controls.Add(this.lblPhone);
            this.panelMainFields.Controls.Add(this.lblContractorType);
            this.panelMainFields.Controls.Add(this.lblMonthlyAmount);
            this.panelMainFields.Controls.Add(this.lblSignalType);
            this.panelMainFields.Controls.Add(this.lblPropertyType);
            this.panelMainFields.Controls.Add(this.lblBuildingType);
            this.panelMainFields.Controls.Add(this.txtContractNumber);
            this.panelMainFields.Controls.Add(this.txtFullName);
            this.panelMainFields.Controls.Add(this.txtAddress);
            this.panelMainFields.Controls.Add(this.txtCadNumber);
            this.panelMainFields.Controls.Add(this.txtEmail);
            this.panelMainFields.Controls.Add(this.txtPhone);
            this.panelMainFields.Controls.Add(this.cmbContractorType);
            this.panelMainFields.Controls.Add(this.txtMonthlyAmount);
            this.panelMainFields.Controls.Add(this.cmbSignalType);
            this.panelMainFields.Controls.Add(this.cmbPropertyType);
            this.panelMainFields.Controls.Add(this.cmbBuildingType);
            this.panelMainFields.Location = new System.Drawing.Point(20, 80);
            this.panelMainFields.Name = "panelMainFields";
            this.panelMainFields.Size = new System.Drawing.Size(1160, 350);
            this.panelMainFields.TabIndex = 3;
            // 
            // lblContractNumber
            // 
            this.lblContractNumber.Location = new System.Drawing.Point(20, 20);
            this.lblContractNumber.Name = "lblContractNumber";
            this.lblContractNumber.Size = new System.Drawing.Size(170, 28);
            this.lblContractNumber.TabIndex = 0;
            this.lblContractNumber.Text = "Номер договора:";
            this.lblContractNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFullName
            // 
            this.lblFullName.Location = new System.Drawing.Point(20, 70);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(170, 28);
            this.lblFullName.TabIndex = 1;
            this.lblFullName.Text = "Наименование/ФИО:";
            this.lblFullName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblAddress
            // 
            this.lblAddress.Location = new System.Drawing.Point(20, 120);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(170, 28);
            this.lblAddress.TabIndex = 2;
            this.lblAddress.Text = "Адрес объекта:";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCadNumber
            // 
            this.lblCadNumber.Location = new System.Drawing.Point(20, 170);
            this.lblCadNumber.Name = "lblCadNumber";
            this.lblCadNumber.Size = new System.Drawing.Size(170, 28);
            this.lblCadNumber.TabIndex = 3;
            this.lblCadNumber.Text = "Кадастровый номер:";
            this.lblCadNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEmail
            // 
            this.lblEmail.Location = new System.Drawing.Point(20, 220);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(170, 28);
            this.lblEmail.TabIndex = 4;
            this.lblEmail.Text = "Email:";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPhone
            // 
            this.lblPhone.Location = new System.Drawing.Point(20, 270);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(170, 28);
            this.lblPhone.TabIndex = 5;
            this.lblPhone.Text = "Телефон:";
            this.lblPhone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblContractorType
            // 
            this.lblContractorType.Location = new System.Drawing.Point(620, 20);
            this.lblContractorType.Name = "lblContractorType";
            this.lblContractorType.Size = new System.Drawing.Size(170, 28);
            this.lblContractorType.TabIndex = 6;
            this.lblContractorType.Text = "Тип контрагента:";
            this.lblContractorType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMonthlyAmount
            // 
            this.lblMonthlyAmount.Location = new System.Drawing.Point(620, 70);
            this.lblMonthlyAmount.Name = "lblMonthlyAmount";
            this.lblMonthlyAmount.Size = new System.Drawing.Size(170, 28);
            this.lblMonthlyAmount.TabIndex = 7;
            this.lblMonthlyAmount.Text = "Сумма в месяц:";
            this.lblMonthlyAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSignalType
            // 
            this.lblSignalType.Location = new System.Drawing.Point(620, 120);
            this.lblSignalType.Name = "lblSignalType";
            this.lblSignalType.Size = new System.Drawing.Size(170, 28);
            this.lblSignalType.TabIndex = 8;
            this.lblSignalType.Text = "Тип сигнализации:";
            this.lblSignalType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPropertyType
            // 
            this.lblPropertyType.Location = new System.Drawing.Point(620, 170);
            this.lblPropertyType.Name = "lblPropertyType";
            this.lblPropertyType.Size = new System.Drawing.Size(170, 28);
            this.lblPropertyType.TabIndex = 9;
            this.lblPropertyType.Text = "Тип недвижимости:";
            this.lblPropertyType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBuildingType
            // 
            this.lblBuildingType.Location = new System.Drawing.Point(620, 220);
            this.lblBuildingType.Name = "lblBuildingType";
            this.lblBuildingType.Size = new System.Drawing.Size(170, 28);
            this.lblBuildingType.TabIndex = 10;
            this.lblBuildingType.Text = "Тип здания:";
            this.lblBuildingType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtContractNumber
            // 
            this.txtContractNumber.Location = new System.Drawing.Point(200, 20);
            this.txtContractNumber.Name = "txtContractNumber";
            this.txtContractNumber.Size = new System.Drawing.Size(350, 25);
            this.txtContractNumber.TabIndex = 0;
            // 
            // txtFullName
            // 
            this.txtFullName.Location = new System.Drawing.Point(200, 70);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(350, 25);
            this.txtFullName.TabIndex = 1;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(200, 120);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(350, 25);
            this.txtAddress.TabIndex = 2;
            // 
            // txtCadNumber
            // 
            this.txtCadNumber.Location = new System.Drawing.Point(200, 170);
            this.txtCadNumber.Name = "txtCadNumber";
            this.txtCadNumber.Size = new System.Drawing.Size(350, 25);
            this.txtCadNumber.TabIndex = 3;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(200, 220);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(350, 25);
            this.txtEmail.TabIndex = 4;
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(200, 270);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(350, 25);
            this.txtPhone.TabIndex = 10;
            // 
            // cmbContractorType
            // 
            this.cmbContractorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbContractorType.FormattingEnabled = true;
            this.cmbContractorType.Location = new System.Drawing.Point(800, 20);
            this.cmbContractorType.Name = "cmbContractorType";
            this.cmbContractorType.Size = new System.Drawing.Size(350, 25);
            this.cmbContractorType.TabIndex = 5;
            // 
            // txtMonthlyAmount
            // 
            this.txtMonthlyAmount.Location = new System.Drawing.Point(800, 70);
            this.txtMonthlyAmount.Name = "txtMonthlyAmount";
            this.txtMonthlyAmount.Size = new System.Drawing.Size(350, 25);
            this.txtMonthlyAmount.TabIndex = 6;
            // 
            // cmbSignalType
            // 
            this.cmbSignalType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSignalType.FormattingEnabled = true;
            this.cmbSignalType.Location = new System.Drawing.Point(800, 120);
            this.cmbSignalType.Name = "cmbSignalType";
            this.cmbSignalType.Size = new System.Drawing.Size(350, 25);
            this.cmbSignalType.TabIndex = 7;
            // 
            // cmbPropertyType
            // 
            this.cmbPropertyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPropertyType.FormattingEnabled = true;
            this.cmbPropertyType.Location = new System.Drawing.Point(800, 170);
            this.cmbPropertyType.Name = "cmbPropertyType";
            this.cmbPropertyType.Size = new System.Drawing.Size(350, 25);
            this.cmbPropertyType.TabIndex = 8;
            // 
            // cmbBuildingType
            // 
            this.cmbBuildingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuildingType.FormattingEnabled = true;
            this.cmbBuildingType.Location = new System.Drawing.Point(800, 220);
            this.cmbBuildingType.Name = "cmbBuildingType";
            this.cmbBuildingType.Size = new System.Drawing.Size(350, 25);
            this.cmbBuildingType.TabIndex = 9;
            // 
            // panelOOO
            // 
            this.panelOOO.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            this.panelOOO.Location = new System.Drawing.Point(20, 440);
            this.panelOOO.Name = "panelOOO";
            this.panelOOO.Size = new System.Drawing.Size(1160, 240);
            this.panelOOO.TabIndex = 4;
            this.panelOOO.Visible = false;
            // 
            // panelIP
            // 
            this.panelIP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            this.panelIP.Location = new System.Drawing.Point(20, 440);
            this.panelIP.Name = "panelIP";
            this.panelIP.Size = new System.Drawing.Size(1160, 240);
            this.panelIP.TabIndex = 5;
            this.panelIP.Visible = false;
            // 
            // panelFL
            // 
            this.panelFL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(255)))));
            this.panelFL.Location = new System.Drawing.Point(20, 440);
            this.panelFL.Name = "panelFL";
            this.panelFL.Size = new System.Drawing.Size(1160, 200);
            this.panelFL.TabIndex = 6;
            this.panelFL.Visible = false;
            // 
            // ContractorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 800); // ИЗМЕНЕНО: увеличена высота до 800
            this.Controls.Add(this.panelFL);
            this.Controls.Add(this.panelIP);
            this.Controls.Add(this.panelOOO);
            this.Controls.Add(this.panelMainFields);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ContractorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Контрагент";
            this.panelHeader.ResumeLayout(false);
            this.panelMainFields.ResumeLayout(false);
            this.panelMainFields.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}