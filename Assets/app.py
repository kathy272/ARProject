from flask import Flask, request, redirect, url_for
import os
import subprocess

app = Flask(__name__)
UPLOAD_FOLDER = 'uploads'
os.makedirs(UPLOAD_FOLDER, exist_ok=True) # Create the upload folder if it doesn't exist
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER

def process_image(image_path):

    subprocess.run(['python', 'automate_conversion.py'])

@app.route('/', methods=['GET', 'POST'])
def upload_file():
    if request.method == 'POST':
        if 'file' not in request.files:
            return redirect(request.url)
        file = request.files['file']
        if file.filename == '':
            return redirect(request.url)
        if file:
            filename = 'colored_map.png'
            file.save(os.path.join(app.config['UPLOAD_FOLDER'], filename))
            process_image(os.path.join(app.config['UPLOAD_FOLDER'], filename))
            print("Image path:", os.path.join(app.config['UPLOAD_FOLDER'], filename))

            return 'File uploaded and processed successfully.'
    return '''
    <!doctype html>
    <title>Upload an Image</title>
    <h1>Upload a map image to convert it to a 3D model</h1>
    <form method=post enctype=multipart/form-data>
      <input type=file name=file>
      <input type=submit value=Upload>
    </form>
    '''

if __name__ == '__main__':
    app.run(debug=True)
