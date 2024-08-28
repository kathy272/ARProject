
import os
import bpy

import sys
def apply_all_transforms():
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)

def create_terrain_from_heightmap(heightmap_path, output_path):
    # Convert relative paths to absolute paths
    heightmap_path = os.path.abspath(heightmap_path)
    output_path = os.path.abspath(output_path)

    # Print paths for debugging
    print("Absolute Heightmap path:", heightmap_path)
    print("Absolute Output path:", output_path)

    # Check if heightmap file exists
    if not os.path.exists(heightmap_path):
        raise FileNotFoundError(f"Heightmap file not found: {heightmap_path}")

    # Clear existing objects
    bpy.ops.object.select_all(action='DESELECT')
    bpy.ops.object.select_by_type(type='MESH')
    bpy.ops.object.delete()

    # Create a new plane with a specific size
    plane_size = 2
    bpy.ops.mesh.primitive_plane_add(size=plane_size, enter_editmode=False)
    plane = bpy.context.object

    # Subdivide the plane to increase mesh resolution
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.subdivide(number_cuts=10)  # Adjust as needed
    bpy.ops.mesh.subdivide(number_cuts=10)  # Further subdivisions
    bpy.ops.mesh.subdivide(number_cuts=2)
    bpy.ops.object.mode_set(mode='OBJECT')

    # Apply displacement modifier
    disp = plane.modifiers.new("Displace", type='DISPLACE')
    tex = bpy.data.textures.new("HeightmapTexture", type='IMAGE')
    tex.image = bpy.data.images.load(heightmap_path)
    disp.texture = tex
    disp.texture_coords = 'LOCAL'
    disp.mid_level = 0.5
    disp.strength = 0.2
    #shade smooth
    bpy.ops.object.shade_smooth()

    # Apply the modifier
    bpy.ops.object.modifier_apply(modifier="Displace")

    # Apply transformations
    apply_all_transforms()
    # Ensure the terrain is flat on the bottom
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')

    # Extrude the mesh to ensure displacement applies to all vertices
    bpy.ops.mesh.extrude_region_move(TRANSFORM_OT_translate={"value": (0, 0, -0.2)})

    # Flatten the terrain
    bpy.ops.transform.resize(value=(1, 1, 0))

    bpy.ops.object.mode_set(mode='OBJECT')
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.mesh.flip_normals()
    bpy.ops.object.mode_set(mode='OBJECT')
    # Export the terrain as an FBX file
    bpy.ops.export_scene.fbx(filepath=output_path, use_selection=True,
                             axis_forward='-Z', axis_up='Y', apply_unit_scale=True)


def main(heightmap_path, output_path):
    # Clear the default scene
    bpy.ops.wm.read_factory_settings(use_empty=True)

    # Create the terrain from the heightmap
    create_terrain_from_heightmap(heightmap_path, output_path)

    # Exit Blender
    bpy.ops.wm.quit_blender()

if __name__ == "__main__":
    import sys

    # Ensure the "--" is in the argument list before trying to access indices
    if "--" in sys.argv:
        heightmap_path = sys.argv[sys.argv.index("--") + 1]
        output_path = sys.argv[sys.argv.index("--") + 2]

        # Call the main function
        main(heightmap_path, output_path)
    else:
        raise ValueError(
            "Missing '--' in arguments. Expected usage: blender --background --python script.py -- <heightmap_path> <output_path>")