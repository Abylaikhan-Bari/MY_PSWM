namespace PasswordManagerApp
{
    partial class LoginPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.loginTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblToRegisterPage = new System.Windows.Forms.Label();
            this.ChkShowPassword2 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Font = new System.Drawing.Font("Javanese Text", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(248, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 36);
            this.label1.TabIndex = 1;
            this.label1.Text = "Password";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordTextBox.Location = new System.Drawing.Point(351, 180);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(170, 35);
            this.passwordTextBox.TabIndex = 4;
            // 
            // loginTextBox
            // 
            this.loginTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loginTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginTextBox.Location = new System.Drawing.Point(351, 112);
            this.loginTextBox.Name = "loginTextBox";
            this.loginTextBox.Size = new System.Drawing.Size(170, 35);
            this.loginTextBox.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.Teal;
            this.button1.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(373, 244);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 51);
            this.button1.TabIndex = 11;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Font = new System.Drawing.Font("Javanese Text", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(248, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 36);
            this.label2.TabIndex = 13;
            this.label2.Text = "Login      ";
            // 
            // lblToRegisterPage
            // 
            this.lblToRegisterPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblToRegisterPage.AutoSize = true;
            this.lblToRegisterPage.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblToRegisterPage.Font = new System.Drawing.Font("Javanese Text", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToRegisterPage.Location = new System.Drawing.Point(335, 310);
            this.lblToRegisterPage.Name = "lblToRegisterPage";
            this.lblToRegisterPage.Size = new System.Drawing.Size(186, 36);
            this.lblToRegisterPage.TabIndex = 16;
            this.lblToRegisterPage.Text = "Not registered? Register";
            this.lblToRegisterPage.Click += new System.EventHandler(this.lblToRegisterPage_Click);
            // 
            // ChkShowPassword2
            // 
            this.ChkShowPassword2.AutoSize = true;
            this.ChkShowPassword2.Location = new System.Drawing.Point(539, 187);
            this.ChkShowPassword2.Name = "ChkShowPassword2";
            this.ChkShowPassword2.Size = new System.Drawing.Size(101, 17);
            this.ChkShowPassword2.TabIndex = 17;
            this.ChkShowPassword2.Text = "Show password";
            this.ChkShowPassword2.UseVisualStyleBackColor = true;
            this.ChkShowPassword2.CheckedChanged += new System.EventHandler(this.ChkShowPassword2_CheckedChanged);
            // 
            // LoginPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ChkShowPassword2);
            this.Controls.Add(this.lblToRegisterPage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.loginTextBox);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.label1);
            this.Name = "LoginPage";
            this.Text = "LoginPage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox loginTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblToRegisterPage;
        private System.Windows.Forms.CheckBox ChkShowPassword2;
    }
}