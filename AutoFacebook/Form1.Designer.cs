namespace AutoFacebook
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Login_Logout = new System.Windows.Forms.GroupBox();
            this.email = new System.Windows.Forms.TextBox();
            this.logout = new System.Windows.Forms.Button();
            this.password = new System.Windows.Forms.TextBox();
            this.login = new System.Windows.Forms.Button();
            this.state = new System.Windows.Forms.TextBox();
            this.start = new System.Windows.Forms.Button();
            this.Login_Logout.SuspendLayout();
            this.SuspendLayout();
            // 
            // Login_Logout
            // 
            this.Login_Logout.Controls.Add(this.email);
            this.Login_Logout.Controls.Add(this.logout);
            this.Login_Logout.Controls.Add(this.password);
            this.Login_Logout.Controls.Add(this.login);
            this.Login_Logout.Location = new System.Drawing.Point(12, 12);
            this.Login_Logout.Name = "Login_Logout";
            this.Login_Logout.Size = new System.Drawing.Size(313, 100);
            this.Login_Logout.TabIndex = 5;
            this.Login_Logout.TabStop = false;
            this.Login_Logout.Text = "1.網頁自動登入登出";
            // 
            // email
            // 
            this.email.Location = new System.Drawing.Point(6, 24);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(201, 25);
            this.email.TabIndex = 2;
            // 
            // logout
            // 
            this.logout.Location = new System.Drawing.Point(225, 69);
            this.logout.Name = "logout";
            this.logout.Size = new System.Drawing.Size(75, 25);
            this.logout.TabIndex = 1;
            this.logout.Text = "登出";
            this.logout.UseVisualStyleBackColor = true;
            this.logout.Click += new System.EventHandler(this.logout_Click);
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(6, 69);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(201, 25);
            this.password.TabIndex = 3;
            // 
            // login
            // 
            this.login.Location = new System.Drawing.Point(225, 24);
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(75, 25);
            this.login.TabIndex = 0;
            this.login.Text = "登入";
            this.login.UseVisualStyleBackColor = true;
            this.login.Click += new System.EventHandler(this.login_Click);
            // 
            // state
            // 
            this.state.Location = new System.Drawing.Point(12, 118);
            this.state.Multiline = true;
            this.state.Name = "state";
            this.state.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.state.Size = new System.Drawing.Size(412, 378);
            this.state.TabIndex = 6;
            // 
            // start
            // 
            this.start.Font = new System.Drawing.Font("新細明體", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.start.Location = new System.Drawing.Point(331, 45);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(93, 43);
            this.start.TabIndex = 7;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 508);
            this.Controls.Add(this.start);
            this.Controls.Add(this.state);
            this.Controls.Add(this.Login_Logout);
            this.Name = "Form1";
            this.Text = "AutoFacebook";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Login_Logout.ResumeLayout(false);
            this.Login_Logout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox Login_Logout;
        private System.Windows.Forms.TextBox email;
        private System.Windows.Forms.Button logout;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Button login;
        private System.Windows.Forms.TextBox state;
        private System.Windows.Forms.Button start;
    }
}

