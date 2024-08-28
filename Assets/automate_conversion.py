import subprocess
import os


def run_blender_script(heightmap_path, output_path):
    blender_executable = r"C:\Program Files\Blender Foundation\Blender 4.0\Blender.exe"
    script_path = "blender_script.py"

    command = [
        blender_executable,
        "--background",  # Run Blender in background mode
        "--python", script_path,
        "--", heightmap_path, output_path
    ]

    # Run the Blender script
    result = subprocess.run(command, capture_output=True, text=True)

    # Print Blender output for debugging
    print("Blender output:\n", result.stdout)
    print("Blender errors:\n", result.stderr)

    # Check if the output file was created
    if not os.path.exists(output_path):
        print("Error: FBX file was not created.")
        return False
    return True


def main():
    input_image = 'uploads/colored_map.png'  # Path to the uploaded map image
    output_model = 'terrain_model.fbx'  # Output path for the 3D






    # Generate heightmap from image
    heightmap_command = f'python generate_heightmap.py {input_image}'
    os.system(heightmap_command)

    # Convert heightmap to 3D mesh
    if run_blender_script('models/heightmap.png', output_model):
        print("3D model created successfully:", output_model)
    else:
        print("Failed to create 3D model.")


if __name__ == "__main__":
    main()
