namespace DatabaseSQLMusicApp
{
    public partial class Form1 : Form
    {
        private BindingSource albumBindingSource = new BindingSource();
        private BindingSource tracksBindingSource = new BindingSource();
        private AlbumDAO albumDao = new AlbumDAO();
        private List<Album> albums = new List<Album>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the data sources
            dataGridView1.DataSource = albumBindingSource;
            dataGridView2.DataSource = tracksBindingSource;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Fetch all albums and bind to the grid
            albums = albumDao.getAllAlbums();
            albumBindingSource.DataSource = albums;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Search for albums by title and bind to the grid
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                albumBindingSource.DataSource = albumDao.searchTitles(textBox1.Text);
            }
            else
            {
                MessageBox.Show("Please enter a title to search.");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle cell click event to display album image and tracks
            if (e.RowIndex >= 0 && dataGridView1.CurrentRow != null)
            {
                var row = dataGridView1.Rows[e.RowIndex];

                if (row.Cells["ID"].Value != null && int.TryParse(row.Cells["ID"].Value.ToString(), out int albumID))
                {
                    tracksBindingSource.DataSource = albumDao.getTracksUsingJoin(albumID);

                    // Load and display album image if URL is available
                    if (row.Cells["ImageURL"].Value != null)
                    {
                        string imageURL = row.Cells["ImageURL"].Value.ToString();
                        try
                        {
                            pictureBox1.Load(imageURL);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error loading image: " + ex.Message);
                        }
                    }
                    else
                    {
                        pictureBox1.Image = null;
                        MessageBox.Show("No image URL available for the selected album.");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid album ID.");
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PictureBox clicked!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Add a new album
            if (string.IsNullOrWhiteSpace(txt_albumName.Text) ||
                string.IsNullOrWhiteSpace(txt_albumArtist.Text) ||
                string.IsNullOrWhiteSpace(txt_albumYear.Text) ||
                string.IsNullOrWhiteSpace(txt_ImageURL.Text) ||
                string.IsNullOrWhiteSpace(txt_description.Text))
            {
                MessageBox.Show("All fields are required to add a new album.");
                return;
            }

            try
            {
                Album album = new Album
                {
                    AlbumName = txt_albumName.Text,
                    ArtistName = txt_albumArtist.Text,
                    Year = int.Parse(txt_albumYear.Text),
                    ImageURL = txt_ImageURL.Text,
                    Description = txt_description.Text
                };

                int result = albumDao.addOneAlbum(album);
                MessageBox.Show($"{result} new row(s) inserted");
                button1_Click(sender, e); // Refresh the albums list
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Please enter a valid year: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            // Optional: Handle label click event if needed
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Handle track deletion and refresh the tracks grid
            if (dataGridView2.CurrentRow != null)
            {
                var trackIDCell = dataGridView2.CurrentRow.Cells["ID"].Value;

                if (trackIDCell != null && int.TryParse(trackIDCell.ToString(), out int trackID))
                {
                    int result = albumDao.deleteTrack(trackID);
                    MessageBox.Show($"Track deleted. Result: {result}");

                    // Refresh the tracks grid for the current album
                    if (dataGridView1.CurrentRow != null)
                    {
                        int albumID = (int)dataGridView1.CurrentRow.Cells["ID"].Value;
                        tracksBindingSource.DataSource = albumDao.getTracksUsingJoin(albumID);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid track ID.");
                }
            }
            else
            {
                MessageBox.Show("No track selected for deletion.");
            }
        }
    }
}
